using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    // Entity/DetalleAsiento.cs
    public class DetalleAsiento
    {
        // Código de la cuenta (E.j., 110505)
        public string CuentaCodigo { get; set; }
        // Cantidad del Debe
        public decimal ValorDebe { get; set; }
        // Cantidad del Haber
        public decimal ValorHaber { get; set; }
    }
}
