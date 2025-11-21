using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ClientWPF.ConectionSockets
{
    public class ConexionSockets
    {

        // Método genérico: Envía cualquier objeto y recibe cualquier respuesta
        public TResponse Enviar<TResponse>(string ip, int puerto, object peticion)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    // Conectar
                    client.Connect(ip, puerto);

                    using (NetworkStream stream = client.GetStream())
                    {
                        // 1. Serializar a JSON y enviar
                        string jsonEnvio = JsonSerializer.Serialize(peticion);
                        byte[] dataEnvio = Encoding.UTF8.GetBytes(jsonEnvio);
                        stream.Write(dataEnvio, 0, dataEnvio.Length);

                        // 2. Leer respuesta
                        byte[] buffer = new byte[4096];
                        int bytesLeidos = stream.Read(buffer, 0, buffer.Length);

                        if (bytesLeidos == 0) return default(TResponse);

                        string jsonRecibido = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                        // 3. Deserializar
                        return JsonSerializer.Deserialize<TResponse>(jsonRecibido);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión con {ip}:{puerto}\n{ex.Message}", "Error de Red");
                return default(TResponse);
            }
        }
    }
}
