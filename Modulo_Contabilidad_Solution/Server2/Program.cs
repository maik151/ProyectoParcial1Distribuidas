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
        string ipStr = config["ConfiguracionServer_Contabilidad:Ip"];
        int puerto = int.Parse(config["ConfiguracionServer_Contabilidad:Puerto"]);
        string cadenaConexion = config["ConnectionStrings:OracleDB"];

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
        Console.WriteLine($"[MODULO CONTABILIDAD] INICIADO");
        Console.WriteLine($"Escuchando en: {ipStr}:{puerto}");
        Console.WriteLine("===================================");

        GestorContabilidad servicio = new GestorContabilidad(cadenaConexion);

        while (true)
        {
            try
            {
                TcpClient client = server.AcceptTcpClient();
                ManejarCliente(client, servicio);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en el servidor: " + ex.Message);
            }
        }
    }

    static void ManejarCliente(TcpClient client, GestorContabilidad servicio)
    {
        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[8192];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string jsonRecibido = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[Petición]: {jsonRecibido}");

                string jsonRespuesta = "";

                try
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonRecibido))
                    {
                        string comando = doc.RootElement.GetProperty("Comando").GetString();

                        switch (comando)
                        {
                            // ========== TIPO CUENTA ==========
                            case "LISTAR_TIPOS_CUENTA":
                                var respListaTipos = servicio.ObtenerTodosTiposCuenta();
                                jsonRespuesta = JsonSerializer.Serialize(respListaTipos);
                                break;

                            case "GUARDAR_TIPO_CUENTA":
                                var reqGuardarTipo = JsonSerializer.Deserialize<TipoCuentaRequest>(jsonRecibido);
                                var respGuardarTipo = servicio.GuardarTipoCuenta(reqGuardarTipo.TipoCuenta);
                                jsonRespuesta = JsonSerializer.Serialize(respGuardarTipo);
                                break;

                            case "ELIMINAR_TIPO_CUENTA":
                                var reqEliminarTipo = JsonSerializer.Deserialize<TipoCuentaRequest>(jsonRecibido);
                                var respEliminarTipo = servicio.EliminarTipoCuenta(reqEliminarTipo.TipoCuenta.TipoCuentaId);
                                jsonRespuesta = JsonSerializer.Serialize(respEliminarTipo);
                                break;

                            // ========== CUENTA ==========
                            case "LISTAR_CUENTAS":
                                var respListaCuentas = servicio.ObtenerTodasCuentas();
                                jsonRespuesta = JsonSerializer.Serialize(respListaCuentas);
                                break;

                            case "GUARDAR_CUENTA":
                                var reqGuardarCuenta = JsonSerializer.Deserialize<CuentaRequest>(jsonRecibido);
                                var respGuardarCuenta = servicio.GuardarCuenta(reqGuardarCuenta.Cuenta);
                                jsonRespuesta = JsonSerializer.Serialize(respGuardarCuenta);
                                break;

                            case "ELIMINAR_CUENTA":
                                var reqEliminarCuenta = JsonSerializer.Deserialize<CuentaRequest>(jsonRecibido);
                                var respEliminarCuenta = servicio.EliminarCuenta(reqEliminarCuenta.Cuenta.CuentaId);
                                jsonRespuesta = JsonSerializer.Serialize(respEliminarCuenta);
                                break;

                            // ========== COMPROBANTE ==========
                            case "LISTAR_COMPROBANTES":
                                var respListaComprobantes = servicio.ObtenerTodosComprobantes();
                                jsonRespuesta = JsonSerializer.Serialize(respListaComprobantes);
                                break;

                            case "GUARDAR_COMPROBANTE":
                                var reqGuardarComp = JsonSerializer.Deserialize<ComprobanteRequest>(jsonRecibido);
                                var respGuardarComp = servicio.GuardarComprobante(reqGuardarComp.Comprobante);
                                jsonRespuesta = JsonSerializer.Serialize(respGuardarComp);
                                break;

                            case "ELIMINAR_COMPROBANTE":
                                var reqEliminarComp = JsonSerializer.Deserialize<ComprobanteRequest>(jsonRecibido);
                                var respEliminarComp = servicio.EliminarComprobante(reqEliminarComp.Comprobante.ComprobanteId);
                                jsonRespuesta = JsonSerializer.Serialize(respEliminarComp);
                                break;

                            default:
                                jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase
                                {
                                    Exito = false,
                                    Mensaje = "Comando no reconocido: " + comando
                                });
                                break;

                            // ========== REPORTES ==========
                            case "REPORTE_BALANCE_GENERAL":
                                var reqBalance = JsonSerializer.Deserialize<ReporteBalanceRequest>(jsonRecibido);
                                var respBalance = servicio.ObtenerBalanceGeneral(reqBalance.FechaInicio, reqBalance.FechaFin);
                                jsonRespuesta = JsonSerializer.Serialize(respBalance);
                                break;

                            case "REPORTE_ESTADO_RESULTADOS":
                                var reqEstado = JsonSerializer.Deserialize<ReporteBalanceRequest>(jsonRecibido);
                                var respEstado = servicio.ObtenerEstadoResultados(reqEstado.FechaInicio, reqEstado.FechaFin);
                                jsonRespuesta = JsonSerializer.Serialize(respEstado);
                                break;

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error procesando: " + ex.Message);
                    jsonRespuesta = JsonSerializer.Serialize(new RespuestaBase
                    {
                        Exito = false,
                        Mensaje = "Error Server: " + ex.Message
                    });
                }

                byte[] msg = Encoding.UTF8.GetBytes(jsonRespuesta);
                stream.Write(msg, 0, msg.Length);
                Console.WriteLine($"[Respuesta enviada]");
            }
        }
        client.Close();
    }

}