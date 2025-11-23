namespace Entity
{
    public class CuentaDTO
    {
        public long CuentaId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int TipoCuentaId { get; set; }
        public string NombreTipoCuenta { get; set; }
        public int Nivel { get; set; }
        public bool EsCuentaMovimiento { get; set; }
    }
}