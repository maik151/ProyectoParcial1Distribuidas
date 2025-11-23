using BussinesEntity;
using Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccesLayer
{
    public class AccesoDatosMantenimiento
    {
        private string connectionString;

        public AccesoDatosMantenimiento(string connString)
        {
            this.connectionString = connString;
        }

        // --- RF-MAN-01: CRUD ACTIVIDADES ---

        public List<ActividadMantenimiento> ListarActividades()
        {
            var lista = new List<ActividadMantenimiento>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // CORREGIDO: Nombre exacto de tabla
                    string sql = "SELECT codigo, nombre FROM ActividadMantenimiento ORDER BY nombre";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ActividadMantenimiento
                                {
                                    Codigo = reader["codigo"].ToString(),
                                    Nombre = reader["nombre"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error DAL Actividades: " + ex.Message); }
            return lista;
        }

        public bool InsertarActividad(ActividadMantenimiento item)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // CORREGIDO: Nombre exacto de tabla
                    string sql = "INSERT INTO ActividadMantenimiento (codigo, nombre) VALUES (:p_cod, :p_nom)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("p_cod", OracleDbType.Varchar2).Value = item.Codigo;
                        cmd.Parameters.Add("p_nom", OracleDbType.Varchar2).Value = item.Nombre;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Insertar Actividad: " + ex.Message); return false; }
        }

        public bool ModificarActividad(ActividadMantenimiento item)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // CORREGIDO: Nombre exacto de tabla
                    string sql = "UPDATE ActividadMantenimiento SET nombre = :p_nom WHERE codigo = :p_cod";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("p_nom", OracleDbType.Varchar2).Value = item.Nombre;
                        cmd.Parameters.Add("p_cod", OracleDbType.Varchar2).Value = item.Codigo;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Update Actividad: " + ex.Message); return false; }
        }

        public bool EliminarActividad(string codigo)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // CORREGIDO: Nombre exacto de tabla
                    string sql = "DELETE FROM ActividadMantenimiento WHERE codigo = :p_cod";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("p_cod", OracleDbType.Varchar2).Value = codigo;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Delete Actividad: " + ex.Message); return false; }
        }

        // --- RF-MAN-02: CRUD ACTIVOS (Referencia Local) ---

        public List<ActivoMantenimiento> ListarActivos()
        {
            var lista = new List<ActivoMantenimiento>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // CORREGIDO: Nombre exacto de tabla
                    string sql = "SELECT activo_id, codigo, nombre, fecha_compra FROM ActivoMantenimiento ORDER BY nombre";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ActivoMantenimiento
                                {
                                    ActivoId = Convert.ToInt64(reader["activo_id"]),
                                    Codigo = reader["codigo"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    FechaCompra = Convert.ToDateTime(reader["fecha_compra"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error DAL Activos: " + ex.Message); }
            return lista;
        }

        public bool InsertarActivo(ActivoMantenimiento item)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // CORREGIDO: Nombre exacto de tabla
                    string sql = "INSERT INTO ActivoMantenimiento (activo_id, codigo, nombre, fecha_compra) " +
                                 "VALUES (:p_id, :p_cod, :p_nom, :p_fecha)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("p_id", OracleDbType.Int64).Value = item.ActivoId;
                        cmd.Parameters.Add("p_cod", OracleDbType.Varchar2).Value = item.Codigo;
                        cmd.Parameters.Add("p_nom", OracleDbType.Varchar2).Value = item.Nombre;
                        cmd.Parameters.Add("p_fecha", OracleDbType.Date).Value = item.FechaCompra;

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Insertar Activo Local: " + ex.Message); return false; }
        }

        // --- RF-MAN-03: TRANSACCIÓN MANTENIMIENTO ---

        public bool GuardarMantenimiento(CabeceraMantenimiento cabecera)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Insertar Cabecera (CORREGIDO: CabeceraMantenimiento)
                    // Nota: Agregué la columna 'numero' que estaba en tu esquema y 'fecha'
                    string sqlCab = "INSERT INTO CabeceraMantenimiento (numero, fecha, responsable) " +
                                    "VALUES (:p_num, :p_fecha, :p_resp) " +
                                    "RETURNING cabecera_mantenimiento_id INTO :out_id";

                    long nuevoId = 0;

                    using (OracleCommand cmd = new OracleCommand(sqlCab, conn))
                    {
                        cmd.Transaction = transaction;
                        cmd.BindByName = true;
                        // Usamos DateTime.Ticks o un GUID corto para el numero si no viene
                        string numeroDoc = string.IsNullOrEmpty(cabecera.Numero) ? DateTime.Now.ToString("yyyyMMddHHmm") : cabecera.Numero;

                        cmd.Parameters.Add("p_num", OracleDbType.Varchar2).Value = numeroDoc;
                        cmd.Parameters.Add("p_fecha", OracleDbType.Date).Value = cabecera.Fecha;
                        cmd.Parameters.Add("p_resp", OracleDbType.Varchar2).Value = cabecera.Responsable;

                        OracleParameter outId = new OracleParameter("out_id", OracleDbType.Int64);
                        outId.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outId);

                        cmd.ExecuteNonQuery();

                        if (outId.Value is Oracle.ManagedDataAccess.Types.OracleDecimal oDec)
                            nuevoId = oDec.ToInt64();
                        else
                            nuevoId = Convert.ToInt64(outId.Value);
                    }

                    // 2. Insertar Detalles (CORREGIDO: DetalleMantenimiento)
                    // Campos: cabecera_mantenimiento_id, activo_id, actividad_codigo, valor
                    string sqlDet = "INSERT INTO DetalleMantenimiento (cabecera_mantenimiento_id, activo_id, actividad_codigo, valor) " +
                                    "VALUES (:p_cab, :p_act, :p_actividad, :p_val)";

                    foreach (var det in cabecera.Detalles)
                    {
                        using (OracleCommand cmd = new OracleCommand(sqlDet, conn))
                        {
                            cmd.Transaction = transaction;
                            cmd.BindByName = true;
                            cmd.Parameters.Add("p_cab", OracleDbType.Int64).Value = nuevoId;
                            cmd.Parameters.Add("p_act", OracleDbType.Int64).Value = det.ActivoId;
                            // CORRECCIÓN IMPORTANTE: Actividad es CODIGO (Varchar2), no ID
                            cmd.Parameters.Add("p_actividad", OracleDbType.Varchar2).Value = det.ActividadCodigo;
                            cmd.Parameters.Add("p_val", OracleDbType.Decimal).Value = det.Valor;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    Console.WriteLine($"[DB] Mantenimiento guardado. ID: {nuevoId}");
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error Transacción: " + ex.Message);
                    return false;
                }
            }
        }

        public List<ReporteGastoDTO> ReporteGastosPorActivo()
        {
            var lista = new List<ReporteGastoDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // Suma agrupada por nombre de activo
                    string sql = @"SELECT a.nombre, SUM(d.valor) as total 
                           FROM DetalleMantenimiento d 
                           JOIN ActivoMantenimiento a ON d.activo_id = a.activo_id 
                           GROUP BY a.nombre
                           ORDER BY total DESC";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ReporteGastoDTO
                                {
                                    NombreActivo = reader["nombre"].ToString(),
                                    // Manejo de nulos por si acaso
                                    TotalGasto = reader["total"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["total"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Reporte 1: " + ex.Message); }
            return lista;
        }

        // REPORTE 2: MATRIZ CRUZADA (DATOS PLANOS)
        public List<ReporteMatrizDTO> ReporteMatrizDatos()
        {
            var lista = new List<ReporteMatrizDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // Traemos Activo - Actividad - Valor (El pivoteo visual se hace en frontend o se muestra plano)
                    string sql = @"SELECT a.nombre as activo, act.nombre as actividad, SUM(d.valor) as valor
                           FROM DetalleMantenimiento d 
                           JOIN ActivoMantenimiento a ON d.activo_id = a.activo_id 
                           JOIN ActividadMantenimiento act ON d.actividad_codigo = act.codigo
                           GROUP BY a.nombre, act.nombre
                           ORDER BY a.nombre";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ReporteMatrizDTO
                                {
                                    Activo = reader["activo"].ToString(),
                                    Actividad = reader["actividad"].ToString(),
                                    Valor = Convert.ToDecimal(reader["valor"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Reporte 2: " + ex.Message); }
            return lista;
        }

        // MODIFICAR ACTIVO
        public bool ModificarActivo(ActivoMantenimiento item)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // Actualizamos nombre y fecha compra basado en el ID
                    string sql = "UPDATE ActivoMantenimiento SET nombre = :p_nom, fecha_compra = :p_fecha, codigo = :p_cod " +
                                 "WHERE activo_id = :p_id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("p_nom", OracleDbType.Varchar2).Value = item.Nombre;
                        cmd.Parameters.Add("p_fecha", OracleDbType.Date).Value = item.FechaCompra;
                        cmd.Parameters.Add("p_cod", OracleDbType.Varchar2).Value = item.Codigo;
                        cmd.Parameters.Add("p_id", OracleDbType.Int64).Value = item.ActivoId;

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Modificar Activo: " + ex.Message);
                return false;
            }
        }

        // ELIMINAR ACTIVO
        public bool EliminarActivo(long id)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM ActivoMantenimiento WHERE activo_id = :p_id";
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("p_id", OracleDbType.Int64).Value = id;
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Eliminar Activo: " + ex.Message);
                return false;
            }
        }
    }
}