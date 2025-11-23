using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Entity;

namespace DataAccesLayer
{
    public class TipoActivoDao
    {
        private readonly AccesoDatos _db;

        public TipoActivoDao(AccesoDatos db)
        {
            _db = db;
        }

        public List<TipoActivoDTO> Listar(string filtroNombre)
        {
            var lista = new List<TipoActivoDTO>();

            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                SELECT CODIGO, NOMBRE
                FROM TIPO_ACTIVO
                WHERE UPPER(NOMBRE) LIKE '%' || UPPER(:filtro) || '%'
                ORDER BY NOMBRE";

            cmd.Parameters.Add(new OracleParameter("filtro", filtroNombre ?? ""));

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new TipoActivoDTO
                {
                    Codigo = dr.GetString(0),
                    Nombre = dr.GetString(1)
                });
            }

            return lista;
        }

        public void Insertar(TipoActivoDTO dto)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO TIPO_ACTIVO (CODIGO, NOMBRE)
                VALUES (:codigo, :nombre)";

            cmd.Parameters.Add("codigo", dto.Codigo);
            cmd.Parameters.Add("nombre", dto.Nombre);

            cmd.ExecuteNonQuery();
        }

        public void Actualizar(TipoActivoDTO dto)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                UPDATE TIPO_ACTIVO
                SET NOMBRE = :nombre
                WHERE CODIGO = :codigo";

            cmd.Parameters.Add("nombre", dto.Nombre);
            cmd.Parameters.Add("codigo", dto.Codigo);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(string codigo)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "DELETE FROM TIPO_ACTIVO WHERE CODIGO = :codigo";
            cmd.Parameters.Add("codigo", codigo);

            cmd.ExecuteNonQuery();
        }
    }
}

