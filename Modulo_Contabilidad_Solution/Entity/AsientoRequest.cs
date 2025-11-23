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
        public long? ReferenciaOrigenId { get; set; } // Añadido para trazar el origen (Cabecera Mantenimiento ID)
        public List<DetalleAsiento> Detalles { get; set; }

    }

    // La respuesta puede reutilizar RespuestaBase
}
