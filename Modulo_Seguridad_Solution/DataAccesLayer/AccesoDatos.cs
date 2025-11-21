using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using BussinessEntity; // Para UsuarioModelo interno
using Entity;          // Para UsuarioDTO externo

namespace DataAccesLayer
{
    public class AccesoDatos
    {
        private string connectionString;

        public AccesoDatos(string connString)
        {
            this.connectionString = connString;
        }

        // --- LOGIN (CORREGIDO PARA LEER ROL REAL) ---
        public UsuarioModelo BuscarUsuarioLogin(string usuario, string clave)
        {
            UsuarioModelo usuarioEncontrado = null;
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine($"[DB] Login: Buscando {usuario}...");

                    // CORRECCIÓN: Hacemos JOIN para obtener el nombre real del rol (1 o 2)
                    string sql = "SELECT u.usuario_id, u.nombre_usuario, u.nombre_completo, r.nombre_rol " +
                                 "FROM Usuarios u " +
                                 "JOIN RolUsuario r ON u.rol_id = r.rol_id " +
                                 "WHERE u.nombre_usuario = :login_user AND u.clave = :login_pass AND u.activo = 'Y'";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("login_user", OracleDbType.Varchar2)).Value = usuario;
                        cmd.Parameters.Add(new OracleParameter("login_pass", OracleDbType.Varchar2)).Value = clave;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuarioEncontrado = new UsuarioModelo
                                {
                                    UsuarioId = Convert.ToInt64(reader["usuario_id"]),
                                    NombreUsuario = reader["nombre_usuario"].ToString(),
                                    NombreCompleto = reader["nombre_completo"].ToString(),
                                    // AHORA SÍ: Leemos el rol real de la BD (ADMIN o USUARIO_COMUN)
                                    NombreRol = reader["nombre_rol"].ToString()
                                };
                                Console.WriteLine($"[DB] Usuario encontrado. Rol: {usuarioEncontrado.NombreRol}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD Login: " + ex.Message);
            }
            return usuarioEncontrado;
        }

        // --- CRUD DE USUARIOS ---

        public List<UsuarioDTO> ListarUsuarios()
        {
            var lista = new List<UsuarioDTO>();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    // JOIN para mostrar datos completos en la tabla
                    string sql = "SELECT u.usuario_id, u.nombre_usuario, u.clave, u.nombre_completo, u.rol_id, r.nombre_rol " +
                                 "FROM Usuarios u JOIN RolUsuario r ON u.rol_id = r.rol_id " +
                                 "WHERE u.activo = 'Y' ORDER BY u.usuario_id ASC";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new UsuarioDTO
                                {
                                    UsuarioId = Convert.ToInt64(reader["usuario_id"]),
                                    NombreUsuario = reader["nombre_usuario"].ToString(),
                                    Clave = reader["clave"].ToString(),
                                    NombreCompleto = reader["nombre_completo"].ToString(),
                                    RolId = Convert.ToInt32(reader["rol_id"]),
                                    NombreRol = reader["nombre_rol"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD Listar: " + ex.Message);
            }
            return lista;
        }

        public bool InsertarUsuario(UsuarioDTO u)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO Usuarios (nombre_usuario, clave, nombre_completo, rol_id) " +
                                 "VALUES (:p_usuario, :p_clave, :p_nombre, :p_rol)";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("p_usuario", OracleDbType.Varchar2)).Value = u.NombreUsuario;
                        cmd.Parameters.Add(new OracleParameter("p_clave", OracleDbType.Varchar2)).Value = u.Clave;
                        cmd.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2)).Value = u.NombreCompleto;
                        cmd.Parameters.Add(new OracleParameter("p_rol", OracleDbType.Int32)).Value = u.RolId; // Aquí enviará 1 o 2

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD Insertar: " + ex.Message);
                return false;
            }
        }

        public bool ModificarUsuario(UsuarioDTO u)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Usuarios SET nombre_usuario=:p_usuario, clave=:p_clave, nombre_completo=:p_nombre, rol_id=:p_rol " +
                                 "WHERE usuario_id=:p_id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("p_usuario", OracleDbType.Varchar2)).Value = u.NombreUsuario;
                        cmd.Parameters.Add(new OracleParameter("p_clave", OracleDbType.Varchar2)).Value = u.Clave;
                        cmd.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2)).Value = u.NombreCompleto;
                        cmd.Parameters.Add(new OracleParameter("p_rol", OracleDbType.Int32)).Value = u.RolId;
                        cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int64)).Value = u.UsuarioId;

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD Modificar: " + ex.Message);
                return false;
            }
        }

        public bool EliminarUsuario(long id)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Usuarios SET activo='N' WHERE usuario_id=:p_id";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleParameter("p_id", id));
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BD Eliminar: " + ex.Message);
                return false;
            }
        }
    }
}