using System.Collections.Generic;
using ClientWPF.ConectionSockets;
using ClientWPF.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class UsuariosController
    {
        ConexionSockets clienteSocket = new ConexionSockets();

        public List<UsuarioDTO> Listar()
        {
            var req = new PeticionGeneral { Comando = "LISTAR_USUARIOS" };

            var resp = clienteSocket.Enviar<ListaUsuariosResponse>(
                ConfigManager.IpSeguridad, ConfigManager.PuertoSeguridad, req);
            return resp?.Usuarios ?? new List<UsuarioDTO>();
        }

        public RespuestaBase Guardar(UsuarioDTO usuario)
        {
            var req = new UsuarioRequest { Comando = "GUARDAR_USUARIO", Usuario = usuario };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpSeguridad, ConfigManager.PuertoSeguridad, req);
        }

        public RespuestaBase Eliminar(long id)
        {
            // Enviamos el objeto solo con el ID para borrar
            var req = new UsuarioRequest { Comando = "ELIMINAR_USUARIO", Usuario = new UsuarioDTO { UsuarioId = id } };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpSeguridad, ConfigManager.PuertoSeguridad, req);
        }
    }
}
