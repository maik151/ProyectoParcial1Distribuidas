using System;
using System.Collections.Generic;
using System.Linq;
using BussinessEntity;
using DataAccesLayer;
using Entity;

namespace BussinessAccessLayer
{
    public class GestorContabilidad
    {
        private AccesoDatosContabilidad dal;

        public GestorContabilidad(string cadenaConexion)
        {
            dal = new AccesoDatosContabilidad(cadenaConexion);
        }

        // ==================== TIPO CUENTA ====================

        public ListaTipoCuentasResponse ObtenerTodosTiposCuenta()
        {
            try
            {
                var lista = dal.ListarTiposCuenta();
                return new ListaTipoCuentasResponse
                {
                    Exito = true,
                    TiposCuenta = lista,
                    Mensaje = "Listado OK"
                };
            }
            catch (Exception ex)
            {
                return new ListaTipoCuentasResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        public RespuestaBase GuardarTipoCuenta(TipoCuentaDTO tc)
        {
            try
            {
                bool ok = false;

                if (tc.TipoCuentaId == 0)
                    ok = dal.InsertarTipoCuenta(tc);
                else
                    ok = dal.ModificarTipoCuenta(tc);

                return new RespuestaBase
                {
                    Exito = ok,
                    Mensaje = ok ? "Guardado correctamente" : "Error al guardar"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaBase
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        public RespuestaBase EliminarTipoCuenta(int id)
        {
            try
            {
                bool ok = dal.EliminarTipoCuenta(id);
                return new RespuestaBase
                {
                    Exito = ok,
                    Mensaje = ok ? "Eliminado correctamente" : "Error al eliminar"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaBase
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }
        // ==================== REPORTES ====================

        public ReporteBalanceResponse ObtenerBalanceGeneral(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var lineas = dal.ObtenerBalanceGeneral(fechaInicio, fechaFin);

                return new ReporteBalanceResponse
                {
                    Exito = true,
                    Mensaje = "Reporte generado correctamente",
                    Lineas = lineas
                };
            }
            catch (Exception ex)
            {
                return new ReporteBalanceResponse
                {
                    Exito = false,
                    Mensaje = "Error al generar reporte: " + ex.Message,
                    Lineas = new List<LineaBalanceDTO>()
                };
            }
        }

        public ReporteEstadoResultadosResponse ObtenerEstadoResultados(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var lineas = dal.ObtenerEstadoResultados(fechaInicio, fechaFin);

                // Calcular utilidad (Ingresos - Gastos)
                decimal totalIngresos = lineas.Where(l => l.Saldo > 0).Sum(l => l.Saldo);
                decimal totalGastos = lineas.Where(l => l.Saldo < 0).Sum(l => Math.Abs(l.Saldo));
                decimal utilidad = totalIngresos - totalGastos;

                return new ReporteEstadoResultadosResponse
                {
                    Exito = true,
                    Mensaje = "Reporte generado correctamente",
                    Lineas = lineas,
                    Utilidad = utilidad
                };
            }
            catch (Exception ex)
            {
                return new ReporteEstadoResultadosResponse
                {
                    Exito = false,
                    Mensaje = "Error al generar reporte: " + ex.Message,
                    Lineas = new List<LineaBalanceDTO>(),
                    Utilidad = 0
                };
            }
        }
        // ==================== CUENTA ====================

        public ListaCuentasResponse ObtenerTodasCuentas()
        {
            try
            {
                var lista = dal.ListarCuentas();
                return new ListaCuentasResponse
                {
                    Exito = true,
                    Cuentas = lista,
                    Mensaje = "Listado OK"
                };
            }
            catch (Exception ex)
            {
                return new ListaCuentasResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        public RespuestaBase GuardarCuenta(CuentaDTO c)
        {
            try
            {
                bool ok = false;

                if (c.CuentaId == 0)
                    ok = dal.InsertarCuenta(c);
                else
                    ok = dal.ModificarCuenta(c);

                return new RespuestaBase
                {
                    Exito = ok,
                    Mensaje = ok ? "Guardado correctamente" : "Error al guardar"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaBase
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        public RespuestaBase EliminarCuenta(long id)
        {
            try
            {
                bool ok = dal.EliminarCuenta(id);
                return new RespuestaBase
                {
                    Exito = ok,
                    Mensaje = ok ? "Eliminado correctamente" : "Error al eliminar"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaBase
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        // ==================== COMPROBANTE ====================

        public ListaComprobantesResponse ObtenerTodosComprobantes()
        {
            try
            {
                var lista = dal.ListarComprobantes();
                return new ListaComprobantesResponse
                {
                    Exito = true,
                    Comprobantes = lista,
                    Mensaje = "Listado OK"
                };
            }
            catch (Exception ex)
            {
                return new ListaComprobantesResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        public RespuestaBase GuardarComprobante(ComprobanteContableDTO comp)
        {
            try
            {
                // VALIDACIÓN: Verificar que el comprobante esté cuadrado
                decimal totalDebe = comp.Detalles.Sum(d => d.CantidadDebe);
                decimal totalHaber = comp.Detalles.Sum(d => d.CantidadHaber);

                if (totalDebe != totalHaber)
                {
                    return new RespuestaBase
                    {
                        Exito = false,
                        Mensaje = $"Asiento descuadrado. Debe: {totalDebe}, Haber: {totalHaber}"
                    };
                }

                bool ok = dal.GuardarComprobante(comp);
                return new RespuestaBase
                {
                    Exito = ok,
                    Mensaje = ok ? "Guardado correctamente" : "Error al guardar"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaBase
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        public RespuestaBase EliminarComprobante(long id)
        {
            try
            {
                bool ok = dal.EliminarComprobante(id);
                return new RespuestaBase
                {
                    Exito = ok,
                    Mensaje = ok ? "Eliminado correctamente" : "Error al eliminar"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaBase
                {
                    Exito = false,
                    Mensaje = ex.Message
                };
            }
        }

        public RespuestaAsientoGuardado GuardarAsiento(AsientoRequest request)
        {
            // 1. VALIDACIÓN DE PARTIDA DOBLE (RF-CON-03)
            decimal sumaDebe = request.Detalles?.Sum(d => d.ValorDebe) ?? 0;
            decimal sumaHaber = request.Detalles?.Sum(d => d.ValorHaber) ?? 0;

            if (sumaDebe != sumaHaber)
            {
                // CORRECCIÓN 1: Devolver el tipo de retorno esperado: RespuestaAsientoGuardado
                return new RespuestaAsientoGuardado
                {
                    Exito = false,
                    Mensaje = $"Error: El asiento NO cuadra. Debe ({sumaDebe:N2}) es diferente de Haber ({sumaHaber:N2})."
                };
            }

            if (request.Detalles == null || request.Detalles.Count == 0)
            {
                // CORRECCIÓN 1: Devolver el tipo de retorno esperado: RespuestaAsientoGuardado
                return new RespuestaAsientoGuardado
                {
                    Exito = false,
                    Mensaje = "Error: El asiento no contiene líneas de detalle."
                };
            }

            try
            {
                // Llama a la DAL para insertar la Cabecera y los Detalles
                long idGenerado = dal.InsertarAsiento(request);

                if (idGenerado > 0)
                {
                    // El asiento se guardó exitosamente.
                    return new RespuestaAsientoGuardado
                    {
                        Exito = true,
                        Mensaje = $"Asiento contable registrado correctamente. ID N° {idGenerado}.",
                        Asiento = new RespuestaAsiento
                        {
                            Numero = (int)idGenerado,
                            Fecha = request.Fecha,
                            Observaciones = request.Glosa
                        }
                    };
                }
                else
                {
                    // Fallo capturado en la DAL (ej: error de conexión o ROLLBACK)
                    return new RespuestaAsientoGuardado { Exito = false, Mensaje = "Fallo al insertar el asiento en la base de datos." };
                }
            }
            catch (Exception ex)
            {
                // CORRECCIÓN 2: Asegurar que todas las rutas de acceso devuelvan un valor.
                // Capturamos el error de la DAL y lo devolvemos en el formato correcto.
                Console.WriteLine($"Error Gestor GuardarAsiento: {ex.Message}");

                return new RespuestaAsientoGuardado
                {
                    Exito = false,
                    Mensaje = $"Error inesperado al procesar el asiento: {ex.Message}"
                };
            }
        }




    }
}