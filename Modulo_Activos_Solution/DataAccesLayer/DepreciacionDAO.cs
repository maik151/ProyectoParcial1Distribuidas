using System;
using Oracle.ManagedDataAccess.Client;
using Entity;

namespace DataAccesLayer
{
    public class DepreciacionDao
    {
        private readonly AccesoDatos _db;

        public DepreciacionDao(AccesoDatos db)
        {
            _db = db;
        }

        public long InsertarCabecera(DepreciacionCabeceraDTO cab)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO DEPRECIACION_CABECERA
                (FECHA_PROCESO, OBSERVACIONES, USUARIO_EJECUCION)
                VALUES (:fecha, :obs, :usr)
                RETURNING DEPRECIACION_ID INTO :id";

            cmd.Parameters.Add("fecha", cab.FechaProceso);
            cmd.Parameters.Add("obs", cab.Observaciones);
            cmd.Parameters.Add("usr", cab.UsuarioEjecucion);

            var idParam = new OracleParameter("id", OracleDbType.Int64)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(idParam);

            cmd.ExecuteNonQuery();

            return Convert.ToInt64(idParam.Value.ToString());
        }

        public void InsertarDetalle(DepreciacionDetalleDTO det)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT INTO DEPRECIACION_DETALLE
                (DEPRECIACION_ID, ACTIVO_ID, PERIODO_NUMERO, VALOR_DEPRECIADO)
                VALUES (:depId, :activoId, :periodo, :valor)";

            cmd.Parameters.Add("depId", det.DepreciacionId);
            cmd.Parameters.Add("activoId", det.ActivoId);
            cmd.Parameters.Add("periodo", det.PeriodoNumero);
            cmd.Parameters.Add("valor", det.ValorDepreciado);

            cmd.ExecuteNonQuery();
        }
    }
}

