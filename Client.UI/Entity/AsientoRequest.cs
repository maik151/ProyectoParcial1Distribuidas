using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class AsientoRequest : PeticionBase
    {
        public DateTime Fecha { get; set; }
        public string Glosa { get; set; }
        public decimal Monto { get; set; }
        public string ModuloOrigen { get; set; } // "MANTENIMIENTO"
    }

    // La respuesta puede reutilizar RespuestaBase
}
