using BussinesEntity;
using DataAccesLayer;
using Entity; // Protocolo compartido
using Microsoft.Extensions.Configuration; // NuGet necesario
using System;
using System.Collections.Generic;
using System.IO; // Necesario para leer configuración
using System.Linq;

namespace BussinessAccessLayer
{
    public class GestorMantenimiento
    {
        private AccesoDatosMantenimiento dal;

        public GestorMantenimiento(string cadenaConexion)
        {
            dal = new AccesoDatosMantenimiento(cadenaConexion);
        }

        // 1. Obtener Actividades para el Combo
        public ListaComboResponse ObtenerActividades()
        {
            try
            {
                var listaDB = dal.ListarActividades();

                // Mapeo Entidad -> DTO
                var listaDTO = listaDB.Select(x => new ItemComboDTO
                {
                    Id = x.Codigo,
                    Descripcion = x.Nombre
                }).ToList();

                return new ListaComboResponse { Exito = true, Items = listaDTO };
            }
            catch (Exception ex)
            {
                return new ListaComboResponse { Exito = false, Mensaje = "Error Actividades: " + ex.Message };
            }
        }

        // 2. Obtener Activos para el Combo
        public ListaComboResponse ObtenerActivos()
        {
            try
            {
                var listaDB = dal.ListarActivos();

                // Mapeo Entidad -> DTO
                var listaDTO = listaDB.Select(x => new ItemComboDTO
                {
                    Id = x.ActivoId.ToString(),
                    Descripcion = x.Nombre
                }).ToList();

                return new ListaComboResponse { Exito = true, Items = listaDTO };
            }
            catch (Exception ex)
            {
                return new ListaComboResponse { Exito = false, Mensaje = "Error Activos: " + ex.Message };
            }
        }

        // 3. Guardar Mantenimiento e INTEGRAR
        public RespuestaBase Guardar(MantenimientoRequest req)
        {
            try
            {
                // A. Mapeo DTO -> Entidad de Negocio
                var cabecera = new CabeceraMantenimiento
                {
                    Numero = req.Numero ?? DateTime.Now.Ticks.ToString().Substring(12), // Generar ID único corto
                    Fecha = req.Fecha,
                    Responsable = req.Responsable,
                    Detalles = req.Detalles.Select(d => new DetalleMantenimiento
                    {
                        ActivoId = d.ActivoId,
                        ActividadCodigo = d.ActividadCodigo,
                        Valor = d.Valor
                    }).ToList()
                };

                // B. Guardar en Oracle Local
                bool guardado = dal.GuardarMantenimiento(cabecera);

                if (guardado)
                {
                    // C. INTEGRACIÓN: Enviar Asiento a Contabilidad (RF-MAN-06)
                    string msgIntegracion = IntegrarConContabilidad(cabecera);

                    return new RespuestaBase
                    {
                        Exito = true,
                        Mensaje = $"Mantenimiento guardado OK. {msgIntegracion}"
                    };
                }
                else
                {
                    return new RespuestaBase { Exito = false, Mensaje = "No se pudo guardar en BD Local." };
                }
            }
            catch (Exception ex)
            {
                return new RespuestaBase { Exito = false, Mensaje = "Error BLL: " + ex.Message };
            }
        }

        // --- MÉTODO PRIVADO DE INTEGRACIÓN ---
        private string IntegrarConContabilidad(CabeceraMantenimiento cab)
        {
            try
            {
                // 1. Calcular total
                decimal totalGasto = cab.Detalles.Sum(d => d.Valor);

                // 2. Leer configuración para saber dónde está Contabilidad
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                var config = builder.Build();

                string ipCont = config["Endpoints:IpContabilidad"];
                string portStr = config["Endpoints:PuertoContabilidad"];

                if (string.IsNullOrEmpty(ipCont) || string.IsNullOrEmpty(portStr))
                    return "(Sin config de Contabilidad)";

                // 3. Crear Petición de Asiento
                var asientoReq = new AsientoRequest
                {
                    Comando = "CREAR_ASIENTO",
                    Fecha = DateTime.Now,
                    Glosa = $"Gasto Mant. #{cab.Numero} Resp: {cab.Responsable}",
                    Monto = totalGasto,
                    ModuloOrigen = "MANTENIMIENTO"
                };

                // 4. Enviar (Usando ClienteSocketInterno)
                var resp = ClienteSocketInterno.EnviarAsiento(ipCont, int.Parse(portStr), asientoReq);

                return resp.Exito ? "(Contabilidad OK)" : $"(Fallo Contabilidad: {resp.Mensaje})";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Integración: " + ex.Message);
                return "(Error conexión Contabilidad)";
            }
        }


        public RespuestaBase GuardarActividad(ActividadDTO dto)
        {
            try
            {
                var entidad = new ActividadMantenimiento { Codigo = dto.Codigo, Nombre = dto.Nombre };

                // Intentamos Insertar
                bool exito = dal.InsertarActividad(entidad);

                // Si falla la inserción (probablemente por PK duplicada), intentamos Modificar
                if (!exito)
                {
                    exito = dal.ModificarActividad(entidad);
                }

                return new RespuestaBase { Exito = exito, Mensaje = exito ? "Actividad Guardada" : "Error al guardar" };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }

        public RespuestaBase EliminarActividad(string codigo)
        {
            try
            {
                bool exito = dal.EliminarActividad(codigo);
                return new RespuestaBase { Exito = exito, Mensaje = exito ? "Actividad Eliminada" : "Error al eliminar (¿Quizás está en uso?)" };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }

        public ReporteResponse ObtenerReporteGastos()
        {
            try
            {
                var datos = dal.ReporteGastosPorActivo();
                return new ReporteResponse { Exito = true, ReporteGastos = datos };
            }
            catch (Exception ex)
            {
                return new ReporteResponse { Exito = false, Mensaje = ex.Message };
            }
        }

        public ReporteResponse ObtenerReporteMatriz()
        {
            try
            {
                var datos = dal.ReporteMatrizDatos();
                return new ReporteResponse { Exito = true, ReporteMatriz = datos };
            }
            catch (Exception ex)
            {
                return new ReporteResponse { Exito = false, Mensaje = ex.Message };
            }
        }


        public RespuestaBase GuardarActivoLocal(long id, string nombre)
        {
            try
            {
                var entidad = new ActivoMantenimiento
                {
                    ActivoId = id,
                    Codigo = "ACT-" + id, // Generamos un código dummy si no viene
                    Nombre = nombre,
                    FechaCompra = DateTime.Now // Fecha dummy
                };

                // 1. Intentar Modificar primero
                bool exito = dal.ModificarActivo(entidad);

                // 2. Si no existe, Insertar
                if (!exito)
                {
                    exito = dal.InsertarActivo(entidad);
                }

                return new RespuestaBase
                {
                    Exito = exito,
                    Mensaje = exito ? "Activo Local Guardado/Actualizado" : "Error al guardar activo"
                };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }

        public RespuestaBase EliminarActivo(long id)
        {
            try
            {
                bool exito = dal.EliminarActivo(id);
                return new RespuestaBase
                {
                    Exito = exito,
                    Mensaje = exito ? "Activo eliminado" : "No se encontró o tiene mantenimientos asociados"
                };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }
    }
}