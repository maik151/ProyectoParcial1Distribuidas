using System;
using System.Collections.Generic;

namespace Entity
{
    public class ComprobanteContableDTO
    {
        public long ComprobanteId { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public string ModuloOrigen { get; set; }
        public long? ReferenciaOrigenId { get; set; }

        public List<DetalleComprobanteDTO> Detalles { get; set; } = new List<DetalleComprobanteDTO>();
    }

    public class DetalleComprobanteDTO
    {
        public long DetalleId { get; set; }
        public long ComprobanteId { get; set; }
        public long CuentaId { get; set; }
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal CantidadDebe { get; set; }
        public decimal CantidadHaber { get; set; }
    }
}