using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    // Entity/AsientoContable.cs
    public class AsientoContable
    {
        // Identificador para edición/eliminación (si aplica, opcional para creación)
        public int AsientoId { get; set; }
        // Cabecera: Número del comprobante (si es manual)
        public string NumeroComprobante { get; set; }
        // Cabecera: Fecha
        public DateTime Fecha { get; set; }
        // Cabecera: Observaciones/Concepto
        public string Observaciones { get; set; }
        // Detalle: Lista de los movimientos de la cuenta
        public List<DetalleAsiento> Detalles { get; set; }
    }
}
