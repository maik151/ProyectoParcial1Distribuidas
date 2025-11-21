using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class LoginResponse : RespuestaBase
    {
        public bool EsValido { get; set; }
        public string NombreUsuario { get; set; }
        public string Rol { get; set; }
    }
}
