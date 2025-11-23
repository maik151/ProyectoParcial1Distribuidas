using BussinesEntity;
using DataAccesLayer;
using Entity; // Protocolo compartido
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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

        // =================================================
        // 1. CATÁLOGOS
        // =================================================

        public ListaComboResponse ObtenerActividades()
        {
            try
            {
                var listaDB = dal.ListarActividades();
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

        public ListaComboResponse ObtenerActivos()
        {
            try
            {
                var listaDB = dal.ListarActivos();
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

        // =================================================
        // 2. CRUD CATÁLOGOS
        // =================================================

        public RespuestaBase GuardarActividad(ActividadDTO dto)
        {
            try
            {
                var entidad = new ActividadMantenimiento
                {
                    Codigo = dto.Codigo.ToUpper(),
                    Nombre = dto.Nombre
                };

                bool exito = dal.ModificarActividad(entidad);
                if (!exito) exito = dal.InsertarActividad(entidad);

                return new RespuestaBase
                {
                    Exito = exito,
                    Mensaje = exito ? "Actividad guardada." : "Error en BD."
                };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }

        public RespuestaBase EliminarActividad(string codigo)
        {
            try
            {
                bool exito = dal.EliminarActividad(codigo);
                return new RespuestaBase
                {
                    Exito = exito,
                    Mensaje = exito ? "Actividad eliminada." : "No se pudo eliminar."
                };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }

        public RespuestaBase GuardarActivoLocal(long id, string nombre)
        {
            try
            {
                var entidad = new ActivoMantenimiento
                {
                    ActivoId = id,
                    Codigo = "ACT-" + id,
                    Nombre = nombre,
                    FechaCompra = DateTime.Now
                };

                bool exito = dal.ModificarActivo(entidad);
                if (!exito) exito = dal.InsertarActivo(entidad);

                return new RespuestaBase { Exito = exito, Mensaje = exito ? "Activo registrado." : "Error." };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }

        public RespuestaBase EliminarActivo(long id)
        {
            try
            {
                bool exito = dal.EliminarActivo(id);
                return new RespuestaBase { Exito = exito, Mensaje = exito ? "Activo eliminado." : "Error." };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = ex.Message }; }
        }

        // =================================================
        // 3. TRANSACCIÓN
        // =================================================

        public RespuestaBase Guardar(MantenimientoRequest req)
        {
            try
            {
                var cabecera = new CabeceraMantenimiento
                {
                    Numero = req.Numero ?? DateTime.Now.Ticks.ToString().Substring(12),
                    Fecha = req.Fecha,
                    Responsable = req.Responsable,
                    Detalles = req.Detalles.Select(d => new DetalleMantenimiento
                    {
                        ActivoId = d.ActivoId,
                        ActividadCodigo = d.ActividadCodigo,
                        Valor = d.Valor
                    }).ToList()
                };

                bool guardado = dal.GuardarMantenimiento(cabecera);

                if (guardado)
                {
                    string msgIntegracion = IntegrarConContabilidad(cabecera);
                    return new RespuestaBase { Exito = true, Mensaje = $"Guardado OK. {msgIntegracion}" };
                }
                return new RespuestaBase { Exito = false, Mensaje = "Error guardando en BD Local." };
            }
            catch (Exception ex) { return new RespuestaBase { Exito = false, Mensaje = "Error BLL: " + ex.Message }; }
        }

        private string IntegrarConContabilidad(CabeceraMantenimiento cab)
        {
            try
            {
                decimal totalGasto = cab.Detalles.Sum(d => d.Valor);

                // 1. OBTENCIÓN DE DATOS PARA LA GLOSA COHERENTE
                string primerActivo = cab.Detalles.FirstOrDefault()?.NombreActivo ?? "Activos Varios";
                string primeraActividad = cab.Detalles.FirstOrDefault()?.NombreActividad ?? "Mantenimiento General";
                int countDetalles = cab.Detalles.Count;

                // Glosa Mejorada: Indica la acción principal, el primer activo y el total de líneas.
                string glosaMejorada = $"Mantenimiento: {primeraActividad} sobre el activo '{primerActivo}' ({countDetalles} líneas). Ref. Comprobante Mantenimiento N° {cab.Numero}. Responsable: {cab.Responsable}.";


                // 2. CONSTRUCCIÓN DE LOS DETALLES DEL ASIENTO (PARTIDA DOBLE)
                var detallesAsiento = new List<DetalleAsiento>();

                // 2.1. Lineas del DEBE (Gasto/Activos)
                foreach (var detalleMantenimiento in cab.Detalles)
                {
                    detallesAsiento.Add(new DetalleAsiento
                    {
                        CuentaCodigo = "5", // Código '5' (Gastos)
                        ValorDebe = detalleMantenimiento.Valor,
                        ValorHaber = 0.00m
                    });
                }

                // 2.2. Linea del HABER (Compensación/Pasivo)
                detallesAsiento.Add(new DetalleAsiento
                {
                    CuentaCodigo = "2.1", // Código '2.1' (Cuentas por Pagar)
                    ValorDebe = 0.00m,
                    ValorHaber = totalGasto
                });


                // 3. CONSTRUCCIÓN DE LA PETICIÓN Y ENVÍO
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var config = builder.Build();

                string ipCont = config["Endpoints:IpContabilidad"];
                string portStr = config["Endpoints:PuertoContabilidad"];

                if (string.IsNullOrEmpty(ipCont)) return "(Sin config Contabilidad)";

                long referenciaId;
                if (!long.TryParse(cab.Numero, out referenciaId))
                {
                    // Manejo de error si el número no es un long válido
                    return "(Error de Integración: El número de comprobante es inválido para ReferenciaOrigenId)";
                }

                var asientoReq = new AsientoRequest
                {
                    Comando = "CREAR_ASIENTO",
                    Fecha = cab.Fecha,
                    Glosa = glosaMejorada,
                    Monto = totalGasto,
                    ModuloOrigen = "MANTENIMIENTO",
                    Detalles = detallesAsiento,

                    // CORRECCIÓN: Asignación explícita del long a la propiedad long?
                    ReferenciaOrigenId = referenciaId // Aquí ya es long, y se asigna a long?
                };
                // LLAMADA Y CASTEO: Hacemos el cast explícito.
                var respBase = ClienteSocketInterno.EnviarAsiento(ipCont, int.Parse(portStr), asientoReq);

                // CORRECCIÓN DEL ERROR DE CONVERSIÓN: Casteamos el tipo base a RespuestaAsientoGuardado
                var resp = respBase as RespuestaAsientoGuardado;

                if (resp == null)
                {
                    return $"(Fallo Cont: Respuesta inesperada del servidor.)";
                }

                // 4. VERIFICACIÓN DE RESULTADO
                // Usamos 'resp' (que es RespuestaAsientoGuardado) para acceder a Exito y Asiento
                return resp.Exito ?
                       $"(Contabilidad OK - Asiento N° {resp.Asiento?.Numero})" :
                       $"(Fallo Cont: {resp.Mensaje})";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de Integración con Contabilidad: {ex.Message}");
                return $"(Error Conexión Contabilidad: {ex.Message})";
            }
        }
        // =================================================
        // 4. REPORTES (CORREGIDO AQUÍ)
        // =================================================

            // Ahora aceptamos ReporteRequest para obtener las fechas
        public ReporteResponse GenerarReporteGastos(ReporteRequest req)
        {
            try
            {
                // AQUÍ PASAMOS LOS ARGUMENTOS QUE FALTABAN
                var datos = dal.ReporteGastosPorActivo(req.FechaInicio, req.FechaFin);

                return new ReporteResponse { Exito = true, ReporteGastos = datos };
            }
            catch (Exception ex)
            {
                return new ReporteResponse { Exito = false, Mensaje = ex.Message };
            }
        }

        public ReporteResponse GenerarReporteMatriz(ReporteRequest req)
        {
            try
            {
                // AQUÍ TAMBIÉN PASAMOS LOS ARGUMENTOS
                var datos = dal.ReporteMatrizDatos(req.FechaInicio, req.FechaFin);

                return new ReporteResponse { Exito = true, ReporteMatriz = datos };
            }
            catch (Exception ex)
            {
                return new ReporteResponse { Exito = false, Mensaje = ex.Message };
            }
        }
    }
}