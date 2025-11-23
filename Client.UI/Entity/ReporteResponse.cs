using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ReporteResponse : RespuestaBase
    {
        public List<ReporteGastoDTO> ReporteGastos { get; set; }
        public List<ReporteMatrizDTO> ReporteMatriz { get; set; }
    }
}
