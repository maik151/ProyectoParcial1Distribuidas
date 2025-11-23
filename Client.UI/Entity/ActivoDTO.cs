using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ActivoDTO
    {
        public long Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCompra { get; set; }
    }
}
