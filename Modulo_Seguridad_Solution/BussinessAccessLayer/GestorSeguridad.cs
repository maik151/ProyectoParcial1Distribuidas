using BussinessEntity;
using DataAccesLayer;

using Entity; // Tu proyecto compartido de Protocolo
namespace BussinessAccessLayer
{
    public class GestorSeguridad
    {
        private AccesoDatos dal;

        public GestorSeguridad(string cadenaConexion)
        {
            dal = new AccesoDatos(cadenaConexion);
        }

        public Entity.LoginResponse ValidarUsuario(LoginRequest peticion)
        {
            // Llamamos a la capa de datos
            var usuario = dal.BuscarUsuarioLogin(peticion.Usuario, peticion.Clave);

            LoginResponse respuesta = new LoginResponse();

            if (usuario != null)
            {
                respuesta.Exito = true;
                respuesta.Mensaje = "Login Correcto";
                respuesta.NombreUsuario = usuario.NombreCompleto;
                respuesta.Rol = usuario.NombreRol;
            }
            else
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Credenciales Incorrectas";
            }

            return respuesta;
        }
    }
}
