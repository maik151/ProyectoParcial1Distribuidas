using System.Collections.Generic;

namespace Entity
{
    public class ListaActivosResponse : RespuestaBase
    {
        public List<ActivoDTO> Activos { get; set; } = new();
    }
}

