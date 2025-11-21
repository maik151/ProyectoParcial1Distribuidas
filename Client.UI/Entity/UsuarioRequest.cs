using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class UsuarioRequest : PeticionBase
    {
        public UsuarioDTO Usuario { get; set; }
    }

    // Respuesta para cuando pedimos la lista de todos
    public class ListaUsuariosResponse : RespuestaBase
    {
        public List<UsuarioDTO> Usuarios { get; set; }
    }
}
