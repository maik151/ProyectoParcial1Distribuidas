using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesEntity
{
    public class CabeceraMantenimiento
    {
        public long CabeceraId { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Responsable { get; set; }
        public string AsientoReferencia { get; set; }
        public List<DetalleMantenimiento> Detalles { get; set; } = new List<DetalleMantenimiento>();
    }
}
