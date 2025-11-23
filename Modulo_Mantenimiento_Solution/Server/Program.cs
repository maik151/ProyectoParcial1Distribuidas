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
        // 1. CARGAR CONFIG
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        IConfiguration config = builder.Build();

        string ipStr = config["ConfiguracionServer_Mantenimiento:Ip"]!;
        int puerto = int.Parse(config["ConfiguracionServer_Mantenimiento:Puerto"]!);
        string cadenaConexion = config["ConnectionStrings:OracleDB"]!;

        // 2. INICIAR SOCKET
        IPAddress ip = IPAddress.Parse(ipStr);
        TcpListener server = new TcpListener(ip, puerto);
        server.Start();

        Console.WriteLine("===================================");
        Console.WriteLine($"[MODULO MANTENIMIENTO] INICIADO");
        Console.WriteLine($"Escuchando en: {ipStr}:{puerto}");
        Console.WriteLine("===================================");

        GestorMantenimiento servicio = new GestorMantenimiento(cadenaConexion);

        while (true)
        {
            try
            {
                TcpClient client = server.AcceptTcpClient();
                ManejarCliente(client, servicio);
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
        }
    }

    static void ManejarCliente(TcpClient client, GestorMantenimiento servicio)
    {
        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[4096]; // Buffer amplio por si mandan muchos detalles
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string jsonRecibido = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[CMD Recibido]: {jsonRecibido}");

                string jsonRespuesta = "";

                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonRecibido))
                    {
                        string comando = doc.RootElement.GetProperty("Comando").GetString();

                        switch (comando)
                        {
                            case "LISTAR_ACTIVIDADES":
                                var respAct = servicio.ObtenerActividades();
                                jsonRespuesta = JsonSerializer.Serialize(respAct);
                                break;

                            case "LISTAR_ACTIVOS":
                                var respActivos = servicio.ObtenerActivos();
                                jsonRespuesta = JsonSerializer.Serialize(respActivos);
                                break;

                            case "GUARDAR_MANTENIMIENTO":
                                var reqMant = JsonSerializer.Deserialize<MantenimientoRequest>(jsonRecibido);
                                var respGuardar = servicio.Guardar(reqMant);
                                jsonRespuesta = JsonSerializer.Serialize(respGuardar);
                                break;
                            

                            case "GUARDAR_ACTIVIDAD":
                                var reqAct = JsonSerializer.Deserialize<ActividadRequest>(jsonRecibido);
                                var respGuardarAct = servicio.GuardarActividad(reqAct.Actividad);
                                jsonRespuesta = JsonSerializer.Serialize(respGuardarAct);
                                break;

                            case "ELIMINAR_ACTIVIDAD":
                                var reqElimAct = JsonSerializer.Deserialize<ActividadRequest>(jsonRecibido);
                                var respElimAct = servicio.EliminarActividad(reqElimAct.Actividad.Codigo);
                                jsonRespuesta = JsonSerializer.Serialize(respElimAct);
                                break;

                            default:
                                jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase { Exito = false, Mensaje = "Comando desconocido en Mantenimiento" });
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error JSON: " + ex.Message);
                    jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase { Exito = false, Mensaje = "Error Server: " + ex.Message });
                }

                byte[] msg = Encoding.UTF8.GetBytes(jsonRespuesta);
                stream.Write(msg, 0, msg.Length);
            }
        }
        client.Close();
    }
}