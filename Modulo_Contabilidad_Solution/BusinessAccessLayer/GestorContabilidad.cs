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
    }
}