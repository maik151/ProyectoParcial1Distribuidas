namespace Entity
{
    public abstract class PeticionBase
    {
        public string Comando { get; set; } // Ej: "VALIDAR_USUARIO"
        public string IdTransaccion { get; set; } = Guid.NewGuid().ToString();
    }
}
