using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesEntity
{
    public class ActivoMantenimiento
    {
        public long ActivoId { get; set; } // PK
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCompra { get; set; }
    }
}
