using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BussinessEntity;

namespace DataAccesLayer
{
    public class AccesoDatos
    {
        
        private string cadenaConexion;


        public AccesoDatos(string conexion)
        {
            cadenaConexion = conexion;
        }

        public UsuarioModelo BuscarUsuarioLogin(string usuario, string clave)
        {
            using (OracleConnection conn = new OracleConnection(cadenaConexion))
            {
                try
                {
                    conn.Open();
                    // Query simple con JOIN para sacar el Rol
                    string sql = @"SELECT u.usuario_id, u.nombre_usuario, u.nombre_completo, r.nombre_rol 
                                 FROM Usuario u JOIN RolUsuario r ON u.rol_id = r.rol_id
                                 WHERE u.nombre_usuario = :user AND u.clave = :pass AND u.activo = 'Y'";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("user", usuario));
                        cmd.Parameters.Add(new OracleParameter("pass", clave));

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UsuarioModelo
                                {
                                    UsuarioId = reader.GetInt64(0),
                                    NombreUsuario = reader.GetString(1),
                                    NombreCompleto = reader.GetString(2),
                                    NombreRol = reader.GetString(3)
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Oracle: " + ex.Message);
                }
            }
            return null; // No encontrado
        }
    }
}
