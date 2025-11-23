using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class DepreciacionDetalleDTO
    {
        public long DepreciacionId { get; set; }
        public long ActivoId { get; set; }
        public short PeriodoNumero { get; set; }
        public decimal ValorDepreciado { get; set; }
    }
}

