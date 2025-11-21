using ClientWPF.Configuration;
using Entity;
using ClientWPF.ConectionSockets;

namespace ClientWPF.Controllers
{
    public class LoginController
    {
        private ConexionSockets clienteSocket;

        public LoginController()
        {
            clienteSocket = new ConexionSockets();
        }

        public LoginResponse Login(string usuario, string clave)
        {
            // 1. Crear el paquete (DTO)
            var request = new LoginRequest
            {
                Comando = "LOGIN", // Debe coincidir con lo que espera el Server
                Usuario = usuario,
                Clave = clave
            };

            // 2. Enviar usando la IP del ConfigManager
            var respuesta = clienteSocket.Enviar<LoginResponse>(
                ConfigManager.IpSeguridad,
                ConfigManager.PuertoSeguridad,
                request
            );

            return respuesta;
        }
    }
}
