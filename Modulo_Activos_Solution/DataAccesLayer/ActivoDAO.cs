using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Entity;

namespace DataAccesLayer
{
    public class ActivoDao
    {
        private readonly AccesoDatos _db;

        public ActivoDao(AccesoDatos db)
        {
            _db = db;
        }

        public List<ActivoDTO> Listar(string filtro)
        {
            var lista = new List<ActivoDTO>();

            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                SELECT ACTIVO_ID, NOMBRE, TIPO_ACTIVO_CODIGO, VALOR_COMPRA, FECHA_ADQUISICION, PERIODOS_DEP_TOTAL
                FROM ACTIVO
                WHERE UPPER(NOMBRE) LIKE '%' || UPPER(:filtro) || '%'
                ORDER BY ACTIVO_ID DESC";

            cmd.Parameters.Add(new OracleParameter("filtro", filtro ?? ""));

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ActivoDTO
                {
                    ActivoId = dr.GetInt64(0),
                    Nombre = dr.GetString(1),
                    TipoActivoCodigo = dr.GetString(2),
                    ValorCompra = dr.GetDecimal(3),
                    FechaAdquisicion = dr.GetDateTime(4),
                    PeriodosDepreciacionTotal = dr.GetInt16(5)
                });
            }

            return lista;
        }

        public void Insertar(ActivoDTO dto)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO ACTIVO
                (NOMBRE, TIPO_ACTIVO_CODIGO, VALOR_COMPRA, FECHA_ADQUISICION, PERIODOS_DEP_TOTAL)
                VALUES (:nombre, :tipo, :valor, :fecha, :periodos)";

            cmd.Parameters.Add("nombre", dto.Nombre);
            cmd.Parameters.Add("tipo", dto.TipoActivoCodigo);
            cmd.Parameters.Add("valor", dto.ValorCompra);
            cmd.Parameters.Add("fecha", dto.FechaAdquisicion);
            cmd.Parameters.Add("periodos", dto.PeriodosDepreciacionTotal);

            cmd.ExecuteNonQuery();
        }

        public void Actualizar(ActivoDTO dto)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                UPDATE ACTIVO SET
                    NOMBRE = :nombre,
                    TIPO_ACTIVO_CODIGO = :tipo,
                    VALOR_COMPRA = :valor,
                    FECHA_ADQUISICION = :fecha,
                    PERIODOS_DEP_TOTAL = :periodos
                WHERE ACTIVO_ID = :id";

            cmd.Parameters.Add("nombre", dto.Nombre);
            cmd.Parameters.Add("tipo", dto.TipoActivoCodigo);
            cmd.Parameters.Add("valor", dto.ValorCompra);
            cmd.Parameters.Add("fecha", dto.FechaAdquisicion);
            cmd.Parameters.Add("periodos", dto.PeriodosDepreciacionTotal);
            cmd.Parameters.Add("id", dto.ActivoId);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(long id)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "DELETE FROM ACTIVO WHERE ACTIVO_ID = :id";
            cmd.Parameters.Add("id", id);

            cmd.ExecuteNonQuery();
        }
    }
}

