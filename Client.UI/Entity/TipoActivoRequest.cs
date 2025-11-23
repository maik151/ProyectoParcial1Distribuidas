using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class TipoActivoRequest : PeticionBase
    {
        public TipoActivoDTO? TipoActivo { get; set; }

        // ✅ necesario para búsqueda en listar
        public string? FiltroNombre { get; set; }
    }
}

