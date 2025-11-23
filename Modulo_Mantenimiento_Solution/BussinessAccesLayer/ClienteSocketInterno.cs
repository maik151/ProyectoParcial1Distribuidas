using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Entity;

namespace BussinessAccessLayer
{
    public static class ClienteSocketInterno
    {
        public static RespuestaBase EnviarAsiento(string ip, int puerto, AsientoRequest asiento)
        {
            try
            {
                using (TcpClient client = new TcpClient(ip, puerto))
                using (NetworkStream stream = client.GetStream())
                {
                    // 1. Serializar
                    string json = JsonSerializer.Serialize(asiento);
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    stream.Write(data, 0, data.Length);

                    // 2. Recibir Confirmación
                    byte[] buffer = new byte[2048];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string jsonResp = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    return JsonSerializer.Deserialize<RespuestaBase>(jsonResp);
                }
            }
            catch (Exception ex)
            {
                // Si falla la conexión con Contabilidad, no rompemos Mantenimiento, solo avisamos.
                return new RespuestaBase { Exito = false, Mensaje = "Error conectando con Contabilidad: " + ex.Message };
            }
        }
    }
}