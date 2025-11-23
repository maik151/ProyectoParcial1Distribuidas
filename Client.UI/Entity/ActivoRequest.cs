namespace Entity
{
    public class ActivoRequest : PeticionBase
    {
        public ActivoDTO? Activo { get; set; }

        // ✅ necesario para Listar con filtro
        public string? FiltroNombre { get; set; }
    }
}

