using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
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

        // 2. LEER VARIABLES
        string ipStr = config["ConfiguracionServer_Seguridad:Ip"]!;
        int puerto = int.Parse(config["ConfiguracionServer_Seguridad:Puerto"]!);
        string cadenaConexion = config["ConnectionStrings:OracleDB"]!;

        if (string.IsNullOrEmpty(ipStr) || string.IsNullOrEmpty(cadenaConexion))
        {
            Console.WriteLine("ERROR CRÍTICO: Configuración incompleta en appsettings.json.");
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
        Console.WriteLine("===================================");

        // Inyectamos la cadena a la capa de Negocio -> Datos
        GestorSeguridad servicio = new GestorSeguridad(cadenaConexion);

        while (true)
        {
            try
            {
                TcpClient client = server.AcceptTcpClient();
                // Procesamos la petición (Idealmente en un Task.Run para concurrencia)
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
                    // Usamos JsonDocument para leer solo el comando sin deserializar todo aún
                    using (JsonDocument doc = JsonDocument.Parse(jsonRecibido))
                    {
                        string comando = doc.RootElement.GetProperty("Comando").GetString();

                        // --- SWITCH DE COMANDOS ---
                        switch (comando)
                        {
                            case "LOGIN":
                                var reqLogin = JsonSerializer.Deserialize<LoginRequest>(jsonRecibido);
                                var respLogin = servicio.ValidarUsuario(reqLogin);
                                jsonRespuesta = JsonSerializer.Serialize(respLogin);
                                break;

                            case "LISTAR_USUARIOS":
                                // Este comando no trae payload, solo ejecutamos la lógica
                                var respLista = servicio.ObtenerTodos();
                                jsonRespuesta = JsonSerializer.Serialize(respLista);
                                break;

                            case "GUARDAR_USUARIO":
                                var reqGuardar = JsonSerializer.Deserialize<UsuarioRequest>(jsonRecibido);
                                // Asumimos que reqGuardar.Usuario trae los datos
                                var respGuardar = servicio.GuardarUsuario(reqGuardar.Usuario);
                                jsonRespuesta = JsonSerializer.Serialize(respGuardar);
                                break;

                            case "ELIMINAR_USUARIO":
                                var reqEliminar = JsonSerializer.Deserialize<UsuarioRequest>(jsonRecibido);
                                // Asumimos que reqEliminar.Usuario.UsuarioId trae el ID a borrar
                                var respEliminar = servicio.EliminarUsuario(reqEliminar.Usuario.UsuarioId);
                                jsonRespuesta = JsonSerializer.Serialize(respEliminar);
                                break;

                            default:
                                jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase { Exito = false, Mensaje = "Comando no reconocido: " + comando });
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error procesando JSON: " + ex.Message);
                    jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase { Exito = false, Mensaje = "Error Interno Server: " + ex.Message });
                }

                // Responder al Cliente
                byte[] msg = Encoding.UTF8.GetBytes(jsonRespuesta);
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine($"[Respuesta Enviada]: {jsonRespuesta}");
            }
        }
        client.Close();
    }
}