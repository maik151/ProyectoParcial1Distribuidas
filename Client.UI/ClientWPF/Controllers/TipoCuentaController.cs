using System.Collections.Generic;
using ClientWPF.ConectionSockets;
using ClientWPF.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class TipoCuentaController
    {
        ConexionSockets clienteSocket = new ConexionSockets();

        public List<TipoCuentaDTO> Listar()
        {
            var req = new PeticionGeneral { Comando = "LISTAR_TIPOS_CUENTA" };
            var resp = clienteSocket.Enviar<ListaTipoCuentasResponse>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
            return resp?.TiposCuenta ?? new List<TipoCuentaDTO>();
        }

        public RespuestaBase Guardar(TipoCuentaDTO tipoCuenta)
        {
            var req = new TipoCuentaRequest
            {
                Comando = "GUARDAR_TIPO_CUENTA",
                TipoCuenta = tipoCuenta
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }

        public RespuestaBase Eliminar(int id)
        {
            var req = new TipoCuentaRequest
            {
                Comando = "ELIMINAR_TIPO_CUENTA",
                TipoCuenta = new TipoCuentaDTO { TipoCuentaId = id }
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }
    }
}