using System.Collections.Generic;
using ClientWPF.ConectionSockets;
using ClientWPF.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class CuentaController
    {
        ConexionSockets clienteSocket = new ConexionSockets();

        public List<CuentaDTO> Listar()
        {
            var req = new PeticionGeneral { Comando = "LISTAR_CUENTAS" };
            var resp = clienteSocket.Enviar<ListaCuentasResponse>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
            return resp?.Cuentas ?? new List<CuentaDTO>();
        }

        public RespuestaBase Guardar(CuentaDTO cuenta)
        {
            
            if (cuenta.EsCuentaMovimiento == null)
            {
                cuenta.EsCuentaMovimiento = true; 
            }

            var req = new CuentaRequest
            {
                Comando = "GUARDAR_CUENTA",
                Cuenta = new CuentaDTO
                {
                    CuentaId = cuenta.CuentaId,
                    Codigo = cuenta.Codigo,
                    Nombre = cuenta.Nombre,
                    TipoCuentaId = cuenta.TipoCuentaId,
                    NombreTipoCuenta = cuenta.NombreTipoCuenta,
                    Nivel = cuenta.Nivel,
                    EsCuentaMovimiento = cuenta.EsCuentaMovimiento  
                }
            };

            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }

        public RespuestaBase Actualizar(CuentaDTO cuenta)
        {
            var req = new CuentaRequest
            {
                Comando = "ACTUALIZAR_CUENTA",
                Cuenta = cuenta
            };

            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }

        public RespuestaBase Eliminar(long id)
        {
            var req = new CuentaRequest
            {
                Comando = "ELIMINAR_CUENTA",
                Cuenta = new CuentaDTO { CuentaId = id }
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }

        public List<TipoCuentaDTO> ObtenerTiposCuenta()
        {
            var req = new PeticionGeneral { Comando = "LISTAR_TIPOS_CUENTA" };
            var resp = clienteSocket.Enviar<ListaTipoCuentasResponse>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
            return resp?.TiposCuenta ?? new List<TipoCuentaDTO>();
        }
    }
}