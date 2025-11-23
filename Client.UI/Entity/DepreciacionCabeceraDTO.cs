using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class DepreciacionCabeceraDTO
    {
        public long DepreciacionId { get; set; }
        public DateTime FechaProceso { get; set; }
        public string Observaciones { get; set; } = "";
        public string UsuarioEjecucion { get; set; } = "";
    }
}

