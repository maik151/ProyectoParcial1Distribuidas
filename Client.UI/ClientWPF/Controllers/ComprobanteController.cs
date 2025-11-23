using System.Collections.Generic;
using ClientWPF.ConectionSockets;
using ClientWPF.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class ComprobanteController
    {
        ConexionSockets clienteSocket = new ConexionSockets();

        public List<ComprobanteContableDTO> Listar()
        {
            var req = new PeticionGeneral { Comando = "LISTAR_COMPROBANTES" };
            var resp = clienteSocket.Enviar<ListaComprobantesResponse>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
            return resp?.Comprobantes ?? new List<ComprobanteContableDTO>();
        }

        public RespuestaBase Guardar(ComprobanteContableDTO comprobante)
        {
            var req = new ComprobanteRequest
            {
                Comando = "GUARDAR_COMPROBANTE",
                Comprobante = comprobante
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }

        public RespuestaBase Eliminar(long id)
        {
            var req = new ComprobanteRequest
            {
                Comando = "ELIMINAR_COMPROBANTE",
                Comprobante = new ComprobanteContableDTO { ComprobanteId = id }
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }
    }
}