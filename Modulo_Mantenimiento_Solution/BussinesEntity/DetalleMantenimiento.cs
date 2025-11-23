using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesEntity
{
    public class DetalleMantenimiento
    {
        // Propiedades originales
        public long ActivoId { get; set; }
        public string ActividadCodigo { get; set; }
        public decimal Valor { get; set; }

        // PROPIEDADES FALTANTES (AÑADIR ESTAS DOS):
        // Estas propiedades deben ser llenadas antes de llamar a IntegrarConContabilidad.
        public string NombreActivo { get; set; }
        public string NombreActividad { get; set; }
    }
}
