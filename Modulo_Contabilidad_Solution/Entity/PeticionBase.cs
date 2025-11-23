using System;

namespace Entity
{
    public abstract class PeticionBase
    {
        public string Comando { get; set; }
        public string IdTransaccion { get; set; } = Guid.NewGuid().ToString();
    }
}