using System;
using System.Collections.Generic;

namespace BussinessEntity
{
    public class ComprobanteModelo
    {
        public long ComprobanteId { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public string ModuloOrigen { get; set; }
        public long? ReferenciaOrigenId { get; set; }

        public List<DetalleComprobanteModelo> Detalles { get; set; } = new List<DetalleComprobanteModelo>();
    }

    public class DetalleComprobanteModelo
    {
        public long DetalleId { get; set; }
        public long ComprobanteId { get; set; }
        public long CuentaId { get; set; }
        public decimal CantidadDebe { get; set; }
        public decimal CantidadHaber { get; set; }
    }
}