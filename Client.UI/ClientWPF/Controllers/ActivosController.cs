using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class ActivosController
    {
        private readonly string _ip;
        private readonly int _puerto;

        public ActivosController()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _ip = config["Activos:IP"] ?? "127.0.0.1";
            _puerto = int.Parse(config["Activos:Puerto"] ?? "9101");
        }

        // ---------------- Helper genérico TCP ----------------
        private TResp Enviar<TResp>(object req)
        {
            using var client = new TcpClient(_ip, _puerto);
            using var stream = client.GetStream();

            string jsonReq = JsonSerializer.Serialize(req);
            byte[] data = Encoding.UTF8.GetBytes(jsonReq);
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[8192];
            int bytes = stream.Read(buffer, 0, buffer.Length);

            if (bytes <= 0)
                throw new Exception("No hubo respuesta del server de Activos.");

            string jsonResp = Encoding.UTF8.GetString(buffer, 0, bytes);

            var resp = JsonSerializer.Deserialize<TResp>(jsonResp);
            if (resp == null)
                throw new Exception("Respuesta inválida del server.");

            return resp;
        }

        // ---------------- TIPO ACTIVO ----------------
        public ListaTipoActivoResponse ListarTipos(string filtroNombre = "")
        {
            var req = new TipoActivoRequest
            {
                Comando = "TA_LISTAR",
                FiltroNombre = filtroNombre
            };
            return Enviar<ListaTipoActivoResponse>(req);
        }

        public RespuestaBase GuardarTipo(TipoActivoDTO dto)
        {
            var req = new TipoActivoRequest
            {
                Comando = "TA_GUARDAR",
                TipoActivo = dto
            };
            return Enviar<RespuestaBase>(req);
        }

        public RespuestaBase EliminarTipo(string codigo)
        {
            var req = new TipoActivoRequest
            {
                Comando = "TA_ELIMINAR",
                TipoActivo = new TipoActivoDTO { Codigo = codigo }
            };
            return Enviar<RespuestaBase>(req);
        }

        // ---------------- ACTIVO ----------------
        public ListaActivosResponse ListarActivos(string filtroNombre = "")
        {
            var req = new ActivoRequest
            {
                Comando = "A_LISTAR",
                FiltroNombre = filtroNombre
            };
            return Enviar<ListaActivosResponse>(req);
        }

        public RespuestaBase GuardarActivo(ActivoDTO dto)
        {
            var req = new ActivoRequest
            {
                Comando = "A_GUARDAR",
                Activo = dto
            };
            return Enviar<RespuestaBase>(req);
        }

        public RespuestaBase EliminarActivo(long id)
        {
            var req = new ActivoRequest
            {
                Comando = "A_ELIMINAR",
                Activo = new ActivoDTO { ActivoId = id }
            };
            return Enviar<RespuestaBase>(req);
        }

        // ---------------- DEPRECIACIÓN ----------------
        public DepreciacionResponse DepreciarMes(DateTime fechaProceso, string observaciones, string usuario)
        {
            var req = new DepreciacionRequest
            {
                Comando = "DEP_MES",
                FechaProceso = fechaProceso,
                Observaciones = observaciones,
                UsuarioEjecucion = usuario
            };
            return Enviar<DepreciacionResponse>(req);
        }
    }
}

