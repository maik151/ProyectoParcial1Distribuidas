using System;

namespace Entity
{
    public class ActivoDTO
    {
        // ✅ Campos reales del módulo Activos
        public long ActivoId { get; set; }
        public string Nombre { get; set; } = "";
        public string TipoActivoCodigo { get; set; } = "";
        public decimal ValorCompra { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public short PeriodosDepreciacionTotal { get; set; }

        // ✅ Alias por compatibilidad con lo anterior
        public long Id
        {
            get => ActivoId;
            set => ActivoId = value;
        }

        public string Codigo
        {
            get => TipoActivoCodigo;
            set => TipoActivoCodigo = value;
        }

        public DateTime FechaCompra
        {
            get => FechaAdquisicion;
            set => FechaAdquisicion = value;
        }
    }
}

