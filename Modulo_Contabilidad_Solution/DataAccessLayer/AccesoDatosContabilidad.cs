using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BussinessEntity;
using Entity;

namespace DataAccesLayer
{
    public class AccesoDatosContabilidad
    {
        private string connectionString;

        public AccesoDatosContabilidad(string connString)
        {
            this.connectionString = connString;
        }

        // ==================== TIPO CUENTA ====================

        public List<TipoCuentaDTO> ListarTiposCuenta()
        {
            var lista = new List<TipoCuentaDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT tipo_cuenta_id, codigo, nombre FROM TipoCuenta ORDER BY tipo_cuenta_id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new TipoCuentaDTO
                                {
                                    TipoCuentaId = Convert.ToInt32(reader["tipo_cuenta_id"]),
                                    Codigo = reader["codigo"].ToString(),
                                    Nombre = reader["nombre"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD ListarTiposCuenta: " + ex.Message);
            }
            return lista;
        }

        public bool InsertarTipoCuenta(TipoCuentaDTO tc)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO TipoCuenta (codigo, nombre) VALUES (:codigo, :nombre)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("codigo", OracleDbType.Varchar2)).Value = tc.Codigo;
                        cmd.Parameters.Add(new OracleParameter("nombre", OracleDbType.Varchar2)).Value = tc.Nombre;

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD InsertarTipoCuenta: " + ex.Message);
                return false;
            }
        }

        public bool ModificarTipoCuenta(TipoCuentaDTO tc)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE TipoCuenta SET codigo=:codigo, nombre=:nombre WHERE tipo_cuenta_id=:id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("codigo", OracleDbType.Varchar2)).Value = tc.Codigo;
                        cmd.Parameters.Add(new OracleParameter("nombre", OracleDbType.Varchar2)).Value = tc.Nombre;
                        cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = tc.TipoCuentaId;

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD ModificarTipoCuenta: " + ex.Message);
                return false;
            }
        }

        public bool EliminarTipoCuenta(int id)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM TipoCuenta WHERE tipo_cuenta_id=:id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32)).Value = id;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD EliminarTipoCuenta: " + ex.Message);
                return false;
            }
        }

        // ==================== CUENTA ====================

        public List<CuentaDTO> ListarCuentas()
        {
            var lista = new List<CuentaDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT c.cuenta_id, c.codigo, c.nombre, c.tipo_cuenta_id, 
                                   tc.nombre as nombre_tipo, c.nivel, c.es_cuenta_movimiento
                                   FROM Cuenta c
                                   JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id
                                   ORDER BY c.codigo";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new CuentaDTO
                                {
                                    CuentaId = Convert.ToInt64(reader["cuenta_id"]),
                                    Codigo = reader["codigo"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    TipoCuentaId = Convert.ToInt32(reader["tipo_cuenta_id"]),
                                    NombreTipoCuenta = reader["nombre_tipo"].ToString(),
                                    Nivel = Convert.ToInt32(reader["nivel"]),
                                    EsCuentaMovimiento = reader["es_cuenta_movimiento"].ToString().ToUpper() == "Y" 
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD ListarCuentas: " + ex.Message);
            }
            return lista;
        }

        public bool InsertarCuenta(CuentaDTO c)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"INSERT INTO Cuenta (codigo, nombre, tipo_cuenta_id, nivel, es_cuenta_movimiento) 
                                   VALUES (:codigo, :nombre, :tipo, :nivel, :movimiento)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("codigo", OracleDbType.Varchar2)).Value = c.Codigo;
                        cmd.Parameters.Add(new OracleParameter("nombre", OracleDbType.Varchar2)).Value = c.Nombre;
                        cmd.Parameters.Add(new OracleParameter("tipo", OracleDbType.Int32)).Value = c.TipoCuentaId;
                        cmd.Parameters.Add(new OracleParameter("nivel", OracleDbType.Int32)).Value = c.Nivel;
                        cmd.Parameters.Add(new OracleParameter("movimiento", OracleDbType.Varchar2)).Value = c.EsCuentaMovimiento ? "Y" : "N"; // ✅ CORREGIDO

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD InsertarCuenta: " + ex.Message);
                return false;
            }
        }

        public bool ModificarCuenta(CuentaDTO c)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"UPDATE Cuenta SET codigo=:codigo, nombre=:nombre, 
                                   tipo_cuenta_id=:tipo, nivel=:nivel, es_cuenta_movimiento=:movimiento 
                                   WHERE cuenta_id=:id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("codigo", OracleDbType.Varchar2)).Value = c.Codigo;
                        cmd.Parameters.Add(new OracleParameter("nombre", OracleDbType.Varchar2)).Value = c.Nombre;
                        cmd.Parameters.Add(new OracleParameter("tipo", OracleDbType.Int32)).Value = c.TipoCuentaId;
                        cmd.Parameters.Add(new OracleParameter("nivel", OracleDbType.Int32)).Value = c.Nivel;
                        cmd.Parameters.Add(new OracleParameter("movimiento", OracleDbType.Varchar2)).Value = c.EsCuentaMovimiento ? "Y" : "N"; 
                        cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = c.CuentaId;

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD ModificarCuenta: " + ex.Message);
                return false;
            }
        }

        public bool EliminarCuenta(long id)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Cuenta WHERE cuenta_id=:id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = id;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD EliminarCuenta: " + ex.Message);
                return false;
            }
        }

        // ==================== COMPROBANTE ====================

        public List<ComprobanteContableDTO> ListarComprobantes()
        {
            var lista = new List<ComprobanteContableDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT comprobante_id, numero, fecha, observaciones, modulo_origen, referencia_origen_id
                                   FROM CabeceraComprobanteCuenta ORDER BY fecha DESC";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ComprobanteContableDTO
                                {
                                    ComprobanteId = Convert.ToInt64(reader["comprobante_id"]),
                                    Numero = reader["numero"].ToString(),
                                    Fecha = Convert.ToDateTime(reader["fecha"]),
                                    Observaciones = reader["observaciones"] == DBNull.Value ? "" : reader["observaciones"].ToString(),
                                    ModuloOrigen = reader["modulo_origen"].ToString(),
                                    ReferenciaOrigenId = reader["referencia_origen_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(reader["referencia_origen_id"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD ListarComprobantes: " + ex.Message);
            }
            return lista;
        }

        public ComprobanteContableDTO ObtenerComprobantePorId(long id)
        {
            ComprobanteContableDTO comprobante = null;
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    string sqlCab = @"SELECT comprobante_id, numero, fecha, observaciones, modulo_origen, referencia_origen_id
                                      FROM CabeceraComprobanteCuenta WHERE comprobante_id = :id";

                    using (OracleCommand cmd = new OracleCommand(sqlCab, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = id;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                comprobante = new ComprobanteContableDTO
                                {
                                    ComprobanteId = Convert.ToInt64(reader["comprobante_id"]),
                                    Numero = reader["numero"].ToString(),
                                    Fecha = Convert.ToDateTime(reader["fecha"]),
                                    Observaciones = reader["observaciones"] == DBNull.Value ? "" : reader["observaciones"].ToString(),
                                    ModuloOrigen = reader["modulo_origen"].ToString(),
                                    ReferenciaOrigenId = reader["referencia_origen_id"] == DBNull.Value ? null : (long?)Convert.ToInt64(reader["referencia_origen_id"])
                                };
                            }
                        }
                    }

                    if (comprobante != null)
                    {
                        string sqlDet = @"SELECT d.detalle_id, d.comprobante_id, d.cuenta_id, d.cantidad_debe, d.cantidad_haber,
                                          c.codigo as codigo_cuenta, c.nombre as nombre_cuenta
                                          FROM DetalleComprobanteCuenta d
                                          JOIN Cuenta c ON d.cuenta_id = c.cuenta_id
                                          WHERE d.comprobante_id = :id";

                        using (OracleCommand cmdDet = new OracleCommand(sqlDet, conn))
                        {
                            cmdDet.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = id;

                            using (OracleDataReader readerDet = cmdDet.ExecuteReader())
                            {
                                while (readerDet.Read())
                                {
                                    comprobante.Detalles.Add(new DetalleComprobanteDTO
                                    {
                                        DetalleId = Convert.ToInt64(readerDet["detalle_id"]),
                                        ComprobanteId = Convert.ToInt64(readerDet["comprobante_id"]),
                                        CuentaId = Convert.ToInt64(readerDet["cuenta_id"]),
                                        CodigoCuenta = readerDet["codigo_cuenta"].ToString(),
                                        NombreCuenta = readerDet["nombre_cuenta"].ToString(),
                                        CantidadDebe = Convert.ToDecimal(readerDet["cantidad_debe"]),
                                        CantidadHaber = Convert.ToDecimal(readerDet["cantidad_haber"])
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD ObtenerComprobantePorId: " + ex.Message);
            }
            return comprobante;
        }

        public bool GuardarComprobante(ComprobanteContableDTO comp)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    OracleTransaction trans = conn.BeginTransaction();

                    try
                    {
                        long comprobanteId = comp.ComprobanteId;

                        if (comprobanteId == 0)
                        {
                            string sqlCab = @"INSERT INTO CabeceraComprobanteCuenta 
                                             (numero, fecha, observaciones, modulo_origen, referencia_origen_id)
                                             VALUES (:numero, :fecha, :obs, :modulo, :ref)
                                             RETURNING comprobante_id INTO :id";

                            using (OracleCommand cmd = new OracleCommand(sqlCab, conn))
                            {
                                cmd.Transaction = trans;
                                cmd.BindByName = true;
                                cmd.Parameters.Add(new OracleParameter("numero", OracleDbType.Varchar2)).Value = comp.Numero;
                                cmd.Parameters.Add(new OracleParameter("fecha", OracleDbType.Date)).Value = comp.Fecha;
                                cmd.Parameters.Add(new OracleParameter("obs", OracleDbType.Varchar2)).Value = comp.Observaciones ?? (object)DBNull.Value;
                                cmd.Parameters.Add(new OracleParameter("modulo", OracleDbType.Varchar2)).Value = comp.ModuloOrigen;
                                cmd.Parameters.Add(new OracleParameter("ref", OracleDbType.Int64)).Value = comp.ReferenciaOrigenId ?? (object)DBNull.Value;

                                OracleParameter outId = new OracleParameter("id", OracleDbType.Int64);
                                outId.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(outId);

                                cmd.ExecuteNonQuery();
                                comprobanteId = Convert.ToInt64(outId.Value.ToString());
                            }

                            foreach (var detalle in comp.Detalles)
                            {
                                string sqlDet = @"INSERT INTO DetalleComprobanteCuenta 
                                                 (comprobante_id, cuenta_id, cantidad_debe, cantidad_haber)
                                                 VALUES (:comp_id, :cuenta_id, :debe, :haber)";

                                using (OracleCommand cmdDet = new OracleCommand(sqlDet, conn))
                                {
                                    cmdDet.Transaction = trans;
                                    cmdDet.BindByName = true;
                                    cmdDet.Parameters.Add(new OracleParameter("comp_id", OracleDbType.Int64)).Value = comprobanteId;
                                    cmdDet.Parameters.Add(new OracleParameter("cuenta_id", OracleDbType.Int64)).Value = detalle.CuentaId;
                                    cmdDet.Parameters.Add(new OracleParameter("debe", OracleDbType.Decimal)).Value = detalle.CantidadDebe;
                                    cmdDet.Parameters.Add(new OracleParameter("haber", OracleDbType.Decimal)).Value = detalle.CantidadHaber;
                                    cmdDet.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            string sqlUpd = @"UPDATE CabeceraComprobanteCuenta 
                                             SET numero=:numero, fecha=:fecha, observaciones=:obs 
                                             WHERE comprobante_id=:id";

                            using (OracleCommand cmd = new OracleCommand(sqlUpd, conn))
                            {
                                cmd.Transaction = trans;
                                cmd.BindByName = true;
                                cmd.Parameters.Add(new OracleParameter("numero", OracleDbType.Varchar2)).Value = comp.Numero;
                                cmd.Parameters.Add(new OracleParameter("fecha", OracleDbType.Date)).Value = comp.Fecha;
                                cmd.Parameters.Add(new OracleParameter("obs", OracleDbType.Varchar2)).Value = comp.Observaciones ?? (object)DBNull.Value;
                                cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = comprobanteId;
                                cmd.ExecuteNonQuery();
                            }

                            string sqlDelDet = "DELETE FROM DetalleComprobanteCuenta WHERE comprobante_id=:id";
                            using (OracleCommand cmdDel = new OracleCommand(sqlDelDet, conn))
                            {
                                cmdDel.Transaction = trans;
                                cmdDel.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = comprobanteId;
                                cmdDel.ExecuteNonQuery();
                            }

                            foreach (var detalle in comp.Detalles)
                            {
                                string sqlDet = @"INSERT INTO DetalleComprobanteCuenta 
                                                 (comprobante_id, cuenta_id, cantidad_debe, cantidad_haber)
                                                 VALUES (:comp_id, :cuenta_id, :debe, :haber)";

                                using (OracleCommand cmdDet = new OracleCommand(sqlDet, conn))
                                {
                                    cmdDet.Transaction = trans;
                                    cmdDet.BindByName = true;
                                    cmdDet.Parameters.Add(new OracleParameter("comp_id", OracleDbType.Int64)).Value = comprobanteId;
                                    cmdDet.Parameters.Add(new OracleParameter("cuenta_id", OracleDbType.Int64)).Value = detalle.CuentaId;
                                    cmdDet.Parameters.Add(new OracleParameter("debe", OracleDbType.Decimal)).Value = detalle.CantidadDebe;
                                    cmdDet.Parameters.Add(new OracleParameter("haber", OracleDbType.Decimal)).Value = detalle.CantidadHaber;
                                    cmdDet.ExecuteNonQuery();
                                }
                            }
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD GuardarComprobante: " + ex.Message);
                return false;
            }
        }

        public bool EliminarComprobante(long id)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    OracleTransaction trans = conn.BeginTransaction();

                    try
                    {
                        string sqlDet = "DELETE FROM DetalleComprobanteCuenta WHERE comprobante_id=:id";
                        using (OracleCommand cmdDet = new OracleCommand(sqlDet, conn))
                        {
                            cmdDet.Transaction = trans;
                            cmdDet.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = id;
                            cmdDet.ExecuteNonQuery();
                        }

                        string sqlCab = "DELETE FROM CabeceraComprobanteCuenta WHERE comprobante_id=:id";
                        using (OracleCommand cmdCab = new OracleCommand(sqlCab, conn))
                        {
                            cmdCab.Transaction = trans;
                            cmdCab.Parameters.Add(new OracleParameter("id", OracleDbType.Int64)).Value = id;
                            cmdCab.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD EliminarComprobante: " + ex.Message);
                return false;
            }
        }

        public List<LineaBalanceDTO> ObtenerBalanceGeneral(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<LineaBalanceDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                SELECT 
                    c.codigo,
                    c.nombre,
                    c.nivel,
                    tc.nombre as tipo_cuenta,
                    COALESCE(SUM(d.cantidad_debe), 0) as total_debe,
                    COALESCE(SUM(d.cantidad_haber), 0) as total_haber
                FROM Cuenta c
                JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id
                LEFT JOIN DetalleComprobanteCuenta d ON c.cuenta_id = d.cuenta_id
                LEFT JOIN CabeceraComprobanteCuenta cab ON d.comprobante_id = cab.comprobante_id
                WHERE cab.fecha BETWEEN :fecha_inicio AND :fecha_fin
                  AND tc.codigo IN ('ACT', 'PAS', 'CAP', 'PAT')
                GROUP BY c.codigo, c.nombre, c.nivel, tc.nombre, tc.codigo
                ORDER BY c.codigo";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("fecha_inicio", OracleDbType.Date)).Value = fechaInicio;
                        cmd.Parameters.Add(new OracleParameter("fecha_fin", OracleDbType.Date)).Value = fechaFin;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                decimal totalDebe = Convert.ToDecimal(reader["total_debe"]);
                                decimal totalHaber = Convert.ToDecimal(reader["total_haber"]);
                                string tipoCuenta = reader["tipo_cuenta"].ToString().ToUpper();

                                decimal saldo = 0;
                                if (tipoCuenta.Contains("ACTIVO"))
                                    saldo = totalDebe - totalHaber;
                                else
                                    saldo = totalHaber - totalDebe;

                                lista.Add(new LineaBalanceDTO
                                {
                                    Codigo = reader["codigo"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Nivel = Convert.ToInt32(reader["nivel"]),
                                    Saldo = saldo
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD Balance General: " + ex.Message);
            }
            return lista;
        }

        public List<LineaBalanceDTO> ObtenerEstadoResultados(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<LineaBalanceDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                SELECT 
                    c.codigo,
                    c.nombre,
                    c.nivel,
                    tc.nombre as tipo_cuenta,
                    COALESCE(SUM(d.cantidad_debe), 0) as total_debe,
                    COALESCE(SUM(d.cantidad_haber), 0) as total_haber
                FROM Cuenta c
                JOIN TipoCuenta tc ON c.tipo_cuenta_id = tc.tipo_cuenta_id
                LEFT JOIN DetalleComprobanteCuenta d ON c.cuenta_id = d.cuenta_id
                LEFT JOIN CabeceraComprobanteCuenta cab ON d.comprobante_id = cab.comprobante_id
                WHERE cab.fecha BETWEEN :fecha_inicio AND :fecha_fin
                  AND tc.codigo IN ('ING', 'EGR', 'GAS', 'COS')
                GROUP BY c.codigo, c.nombre, c.nivel, tc.nombre, tc.codigo
                ORDER BY c.codigo";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("fecha_inicio", OracleDbType.Date)).Value = fechaInicio;
                        cmd.Parameters.Add(new OracleParameter("fecha_fin", OracleDbType.Date)).Value = fechaFin;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                decimal totalDebe = Convert.ToDecimal(reader["total_debe"]);
                                decimal totalHaber = Convert.ToDecimal(reader["total_haber"]);
                                string tipoCuenta = reader["tipo_cuenta"].ToString().ToUpper();

                                decimal saldo = 0;
                                if (tipoCuenta.Contains("INGRESO"))
                                    saldo = totalHaber - totalDebe;
                                else
                                    saldo = totalDebe - totalHaber;

                                lista.Add(new LineaBalanceDTO
                                {
                                    Codigo = reader["codigo"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Nivel = Convert.ToInt32(reader["nivel"]),
                                    Saldo = saldo
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD Estado Resultados: " + ex.Message);
            }
            return lista;
        }


        public long InsertarAsiento(AsientoRequest asientoRequest)
        {
            long comprobanteId = 0;

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    OracleTransaction trans = conn.BeginTransaction();
                    string numeroTemp = DateTime.Now.ToString("yyyyMMddHHmmss");

                    try
                    {
                        // 1. INSERTAR CABECERA y OBTENER EL ID GENERADO
                        string sqlCab = @"INSERT INTO CabeceraComprobanteCuenta
                                     (numero, fecha, observaciones, modulo_origen, referencia_origen_id) 
                                     VALUES (:numero, :fecha, :obs, :modulo, :ref)
                                     RETURNING comprobante_id INTO :id";

                        using (OracleCommand cmd = new OracleCommand(sqlCab, conn))
                        {
                            cmd.Transaction = trans;
                            cmd.BindByName = true;
                            cmd.Parameters.Add(new OracleParameter("numero", OracleDbType.Varchar2)).Value = numeroTemp;
                            // Parámetros de la Cabecera
                            cmd.Parameters.Add(new OracleParameter("fecha", OracleDbType.Date)).Value = asientoRequest.Fecha;
                            cmd.Parameters.Add(new OracleParameter("obs", OracleDbType.Varchar2)).Value = asientoRequest.Glosa ?? (object)DBNull.Value;
                            cmd.Parameters.Add(new OracleParameter("modulo", OracleDbType.Varchar2)).Value = asientoRequest.ModuloOrigen;

                            // Usamos el ID de referencia si existe
                            cmd.Parameters.Add(new OracleParameter("ref", OracleDbType.Int64)).Value = asientoRequest.ReferenciaOrigenId ?? (object)DBNull.Value;

                            OracleParameter outId = new OracleParameter("id", OracleDbType.Int64);
                            outId.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(outId);

                            cmd.ExecuteNonQuery();
                            comprobanteId = Convert.ToInt64(outId.Value.ToString());
                        }

                        // 2. INSERTAR DETALLES
                        string sqlDet = @"INSERT INTO DetalleComprobanteCuenta
                          (comprobante_id, cuenta_id, cantidad_debe, cantidad_haber)
                          VALUES (:comp_id, :cuenta_id, :debe, :haber)"; // <--- CORRECCIÓN: Ahora se usa cuenta_id

                        foreach (var detalle in asientoRequest.Detalles)
                        {
                            // OBTENER EL ID DE CUENTA DENTRO DE LA TRANSACCIÓN
                            long cuentaId = ObtenerCuentaIdPorCodigo(detalle.CuentaCodigo, conn, trans);

                            if (cuentaId == 0)
                            {
                                // Si la cuenta no existe, forzamos un error de transacción para ROLLBACK
                                throw new Exception($"El código de cuenta '{detalle.CuentaCodigo}' no existe en el Plan de Cuentas.");
                            }

                            using (OracleCommand cmdDet = new OracleCommand(sqlDet, conn))
                            {
                                cmdDet.Transaction = trans;
                                cmdDet.BindByName = true;
                                cmdDet.Parameters.Add(new OracleParameter("comp_id", OracleDbType.Int64)).Value = comprobanteId;

                                // CORRECCIÓN FINAL: Bindear el ID numérico
                                cmdDet.Parameters.Add(new OracleParameter("cuenta_id", OracleDbType.Int64)).Value = cuentaId;

                                cmdDet.Parameters.Add(new OracleParameter("debe", OracleDbType.Decimal)).Value = detalle.ValorDebe;
                                cmdDet.Parameters.Add(new OracleParameter("haber", OracleDbType.Decimal)).Value = detalle.ValorHaber;

                                cmdDet.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                    }
                    catch (Exception txEx)
                    {
                        trans.Rollback();
                        // Relanzamos la excepción para que el GestorContabilidad sepa el error
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD InsertarAsiento: " + ex.Message);
                return 0;
            }
            return comprobanteId; // Devolvemos el ID generado
        }


        public long ObtenerCuentaIdPorCodigo(string codigo, OracleConnection conn, OracleTransaction trans)
        {
            long cuentaId = 0;

            // NOTA: Usamos la conexión y transacción existentes para mantener la atomicidad.
            string sql = "SELECT cuenta_id FROM Cuenta WHERE codigo = :codigo";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                cmd.Transaction = trans; // Usar la transacción actual
                cmd.BindByName = true;
                cmd.Parameters.Add(new OracleParameter("codigo", OracleDbType.Varchar2)).Value = codigo;

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    cuentaId = Convert.ToInt64(result);
                }
            }
            return cuentaId;
        }
    }
}