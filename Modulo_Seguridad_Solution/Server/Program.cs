using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration; // NuGet: Microsoft.Extensions.Configuration.Json
using BussinessAccessLayer;
using Entity;

class Program
{
    static void Main(string[] args)
    {
        // 1. CARGAR LA CONFIGURACIÓN
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        // 2. LEER TUS VARIABLES EXACTAS
        // Nota el uso de los dos puntos (:) para navegar en el JSON
        string ipStr = config["ConfiguracionServer_Seguridad:Ip"];
        int puerto = int.Parse(config["ConfiguracionServer_Seguridad:Puerto"]);
        string cadenaConexion = config["ConnectionStrings:OracleDB"];

        // Validación rápida por si el JSON está mal
        if (string.IsNullOrEmpty(ipStr) || string.IsNullOrEmpty(cadenaConexion))
        {
            Console.WriteLine("ERROR CRÍTICO: No se pudo leer la IP o la Cadena de Conexión del JSON.");
            Console.ReadKey();
            return;
        }

        // 3. INICIAR EL SERVIDOR
        IPAddress ip = IPAddress.Parse(ipStr);
        TcpListener server = new TcpListener(ip, puerto);
        server.Start();

        Console.WriteLine("===================================");
        Console.WriteLine($"[MODULO SEGURIDAD] INICIADO");
        Console.WriteLine($"Escuchando en: {ipStr}:{puerto}");
        Console.WriteLine($"Conectando a BD con Wallet en: C:\\OracleWallet"); // Solo informativo
        Console.WriteLine("===================================");

        // Inyectamos la cadena a la capa de Negocio -> Datos
        GestorSeguridad servicio = new GestorSeguridad(cadenaConexion);

        while (true)
        {
            try
            {
                TcpClient client = server.AcceptTcpClient();
                // Procesamos la petición
                ManejarCliente(client, servicio);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en el servidor: " + ex.Message);
            }
        }
    }

    static void ManejarCliente(TcpClient client, GestorSeguridad servicio)
    {
        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[4096]; 
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string jsonRecibido = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[Petición Recibida]: {jsonRecibido}");

                string jsonRespuesta = "";

                try
                {
                    // Deserializar comando base
                    var peticionBase = JsonSerializer.Deserialize<PeticionBase>(jsonRecibido);

                    if (peticionBase != null && peticionBase.Comando == "LOGIN")
                    {
                        var request = JsonSerializer.Deserialize<LoginRequest>(jsonRecibido);
                        var response = servicio.ValidarUsuario(request);
                        jsonRespuesta = JsonSerializer.Serialize(response);
                    }
                    else
                    {
                        jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase { Exito = false, Mensaje = "Comando no reconocido" });
                    }
                }
                catch (Exception ex)
                {
                    jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase { Exito = false, Mensaje = "Error procesando JSON: " + ex.Message });
                }

                // Responder
                byte[] msg = Encoding.UTF8.GetBytes(jsonRespuesta);
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine($"[Respuesta Enviada]: {jsonRespuesta}");
            }
        }
        client.Close();
    }
}