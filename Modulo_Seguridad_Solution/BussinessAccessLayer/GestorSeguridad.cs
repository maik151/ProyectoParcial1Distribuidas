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

        public ListaUsuariosResponse ObtenerTodos()
        {
            try
            {
                var lista = dal.ListarUsuarios();
                return new ListaUsuariosResponse { Exito = true, Usuarios = lista, Mensaje = "Listado OK" };
            }
            catch (Exception ex)
            {
                return new ListaUsuariosResponse { Exito = false, Mensaje = ex.Message };
            }
        }

        public RespuestaBase GuardarUsuario(UsuarioDTO u)
        {
            try
            {
                bool ok = false;
                if (u.UsuarioId == 0) // Si ID es 0, es NUEVO
                    ok = dal.InsertarUsuario(u);
                else // Si tiene ID, es MODIFICAR
                    ok = dal.ModificarUsuario(u);

                return new RespuestaBase { Exito = ok, Mensaje = ok ? "Guardado correctamente" : "Error al guardar" };
            }
            catch (Exception ex)
            {
                return new RespuestaBase { Exito = false, Mensaje = ex.Message };
            }
        }

        public RespuestaBase EliminarUsuario(long id)
        {
            try
            {
                bool ok = dal.EliminarUsuario(id);
                return new RespuestaBase { Exito = ok, Mensaje = ok ? "Eliminado correctamente" : "Error al eliminar" };
            }
            catch (Exception ex)
            {
                return new RespuestaBase { Exito = false, Mensaje = ex.Message };
            }
        }

    }
}
