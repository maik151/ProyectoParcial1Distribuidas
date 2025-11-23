using System;
using ClientWPF.ConectionSockets;
using ClientWPF.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class ReporteController
    {
        ConexionSockets clienteSocket = new ConexionSockets();

        public ReporteBalanceResponse ObtenerBalanceGeneral(DateTime fechaInicio, DateTime fechaFin)
        {
            var req = new ReporteBalanceRequest
            {
                Comando = "REPORTE_BALANCE_GENERAL",
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            return clienteSocket.Enviar<ReporteBalanceResponse>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }

        public ReporteEstadoResultadosResponse ObtenerEstadoResultados(DateTime fechaInicio, DateTime fechaFin)
        {
            var req = new ReporteBalanceRequest
            {
                Comando = "REPORTE_ESTADO_RESULTADOS",
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            return clienteSocket.Enviar<ReporteEstadoResultadosResponse>(
                ConfigManager.IpContabilidad,
                ConfigManager.PuertoContabilidad,
                req);
        }
    }
}