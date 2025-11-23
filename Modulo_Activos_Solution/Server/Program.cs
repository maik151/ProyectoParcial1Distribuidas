using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Entity;
using BusinessAccessLayer;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string ip = config["Servidor:IP"] ?? "127.0.0.1";
            int puerto = int.Parse(config["Servidor:Puerto"] ?? "9101");

            var gestor = new GestorActivos(config);

            var listener = new TcpListener(IPAddress.Parse(ip), puerto);
            listener.Start();

            Console.WriteLine($"[ACTIVOS SERVER] Escuchando en {ip}:{puerto}");

            while (true)
            {
                using var client = listener.AcceptTcpClient();
                using var stream = client.GetStream();

                byte[] buffer = new byte[8192];
                int bytes = stream.Read(buffer, 0, buffer.Length);

                if (bytes <= 0) continue;

                string jsonReq = Encoding.UTF8.GetString(buffer, 0, bytes);

                string jsonResp = Procesar(jsonReq, gestor);

                byte[] dataResp = Encoding.UTF8.GetBytes(jsonResp);
                stream.Write(dataResp, 0, dataResp.Length);
            }
        }

        static string Procesar(string jsonReq, GestorActivos gestor)
        {
            // Solo necesitamos leer el Comando base
            var baseReq = JsonSerializer.Deserialize<PeticionBase>(jsonReq);

            if (baseReq == null || string.IsNullOrWhiteSpace(baseReq.Comando))
                return JsonSerializer.Serialize(new RespuestaBase
                {
                    Exito = false,
                    Mensaje = "Comando inválido"
                });

            switch (baseReq.Comando)
            {
                // ----- TIPOS -----
                case "TA_LISTAR":
                    var taListReq = JsonSerializer.Deserialize<TipoActivoRequest>(jsonReq);
                    return JsonSerializer.Serialize(gestor.ListarTipos(taListReq!));

                case "TA_GUARDAR":
                    var taSaveReq = JsonSerializer.Deserialize<TipoActivoRequest>(jsonReq);
                    return JsonSerializer.Serialize(gestor.GuardarTipo(taSaveReq!));

                case "TA_ELIMINAR":
                    var taDelReq = JsonSerializer.Deserialize<TipoActivoRequest>(jsonReq);
                    return JsonSerializer.Serialize(gestor.EliminarTipo(taDelReq!));

                // ----- ACTIVOS -----
                case "A_LISTAR":
                    var aListReq = JsonSerializer.Deserialize<ActivoRequest>(jsonReq);
                    return JsonSerializer.Serialize(gestor.ListarActivos(aListReq!));

                case "A_GUARDAR":
                    var aSaveReq = JsonSerializer.Deserialize<ActivoRequest>(jsonReq);
                    return JsonSerializer.Serialize(gestor.GuardarActivo(aSaveReq!));

                case "A_ELIMINAR":
                    var aDelReq = JsonSerializer.Deserialize<ActivoRequest>(jsonReq);
                    return JsonSerializer.Serialize(gestor.EliminarActivo(aDelReq!));

                // ----- DEPRECIACIÓN -----
                case "DEP_MES":
                    var depReq = JsonSerializer.Deserialize<DepreciacionRequest>(jsonReq);
                    return JsonSerializer.Serialize(gestor.DepreciarMes(depReq!));

                default:
                    return JsonSerializer.Serialize(new RespuestaBase
                    {
                        Exito = false,
                        Mensaje = $"Comando no reconocido: {baseReq.Comando}"
                    });
            }
        }
    }
}
