using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class UsuarioDTO
    {
        public long UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public string NombreCompleto { get; set; }
        public int RolId { get; set; }
        public string NombreRol { get; set; } // Para mostrar en la grilla
    }
}