using System.Collections.Generic;

namespace Entity
{
    // ========== TIPO CUENTA ==========
    public class TipoCuentaRequest : PeticionBase
    {
        public TipoCuentaDTO TipoCuenta { get; set; }
    }

    public class ListaTipoCuentasResponse : RespuestaBase
    {
        public List<TipoCuentaDTO> TiposCuenta { get; set; }
    }

    // ========== CUENTA ==========
    public class CuentaRequest : PeticionBase
    {
        public CuentaDTO Cuenta { get; set; }
    }

    public class ListaCuentasResponse : RespuestaBase
    {
        public List<CuentaDTO> Cuentas { get; set; }  // ✅ CORREGIDO
    }

    // ========== COMPROBANTE ==========
    public class ComprobanteRequest : PeticionBase
    {
        public ComprobanteContableDTO Comprobante { get; set; }
    }

    public class ListaComprobantesResponse : RespuestaBase
    {
        public List<ComprobanteContableDTO> Comprobantes { get; set; }
    }

    // ========== REPORTES ==========
    public class ReporteBalanceRequest : PeticionBase
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class ReporteBalanceResponse : RespuestaBase
    {
        public List<LineaBalanceDTO> Lineas { get; set; }
    }

    public class LineaBalanceDTO
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Nivel { get; set; }
        public decimal Saldo { get; set; }
    }

    public class ReporteEstadoResultadosResponse : RespuestaBase
    {
        public List<LineaBalanceDTO> Lineas { get; set; }
        public decimal Utilidad { get; set; }
    }
}