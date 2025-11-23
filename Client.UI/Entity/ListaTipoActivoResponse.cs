using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ListaTipoActivoResponse : RespuestaBase
    {
        public List<TipoActivoDTO> Tipos { get; set; } = new();
    }
}
