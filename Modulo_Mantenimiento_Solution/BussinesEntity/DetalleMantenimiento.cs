using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesEntity
{
    public class DetalleMantenimiento
    {
        public long ActivoId { get; set; }
        public string ActividadCodigo { get; set; }
        public decimal Valor { get; set; }
    }
}
