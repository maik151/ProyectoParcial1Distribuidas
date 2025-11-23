using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;

namespace DataAccesLayer
{
    public class AccesoDatos
    {
        private readonly string _connectionString;

        public AccesoDatos(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("OracleConnection")
                                ?? throw new Exception("Cadena OracleConnection no encontrada.");
        }

        public OracleConnection GetConnection()
        {
            var conn = new OracleConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
