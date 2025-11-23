using System.Collections.Generic;
using ClientWPF.ConectionSockets;
using ClientWPF.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class MantenimientoController
    {
        ConexionSockets clienteSocket = new ConexionSockets();

        // =================================================
        // 1. LISTAR DATOS (Para Combos y Tablas)
        // =================================================
        public List<ItemComboDTO> ListarActividades()
        {
            var req = new PeticionGeneral { Comando = "LISTAR_ACTIVIDADES" };
            var resp = clienteSocket.Enviar<ListaComboResponse>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
            return resp?.Items ?? new List<ItemComboDTO>();
        }

        public List<ItemComboDTO> ListarActivos()
        {
            var req = new PeticionGeneral { Comando = "LISTAR_ACTIVOS" };
            var resp = clienteSocket.Enviar<ListaComboResponse>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
            return resp?.Items ?? new List<ItemComboDTO>();
        }

        // =================================================
        // 2. CRUD ACTIVIDADES (RF-MAN-01)
        // =================================================
        public RespuestaBase GuardarActividad(string codigo, string nombre)
        {
            var req = new ActividadRequest
            {
                Comando = "GUARDAR_ACTIVIDAD",
                Actividad = new ActividadDTO { Codigo = codigo, Nombre = nombre }
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
        }

        public RespuestaBase EliminarActividad(string codigo)
        {
            var req = new ActividadRequest
            {
                Comando = "ELIMINAR_ACTIVIDAD",
                Actividad = new ActividadDTO { Codigo = codigo }
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
        }

        // =================================================
        // 3. CRUD ACTIVOS (RF-MAN-02)
        // =================================================
        public RespuestaBase GuardarActivo(long id, string nombre)
        {
            var req = new ActivoRequest
            {
                Comando = "GUARDAR_ACTIVO",
                Activo = new ActivoDTO { Id = id, Nombre = nombre }
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
        }

        public RespuestaBase EliminarActivo(long id)
        {
            var req = new ActivoRequest
            {
                Comando = "ELIMINAR_ACTIVO",
                Activo = new ActivoDTO { Id = id }
            };
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
        }

        // =================================================
        // 4. TRANSACCIÓN PRINCIPAL (RF-MAN-03)
        // =================================================
        public RespuestaBase GuardarMantenimiento(MantenimientoRequest transaccion)
        {
            transaccion.Comando = "GUARDAR_MANTENIMIENTO";
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, transaccion);
        }

        // =================================================
        // 5. REPORTES (RF-MAN-04/05)
        // =================================================
        public List<ReporteGastoDTO> ObtenerReporteGastos()
        {
            var req = new PeticionGeneral { Comando = "REPORTE_GASTOS" };
            var resp = clienteSocket.Enviar<ReporteResponse>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
            return resp?.ReporteGastos ?? new List<ReporteGastoDTO>();
        }

        public List<ReporteMatrizDTO> ObtenerReporteMatriz()
        {
            var req = new PeticionGeneral { Comando = "REPORTE_MATRIZ" };
            var resp = clienteSocket.Enviar<ReporteResponse>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);
            return resp?.ReporteMatriz ?? new List<ReporteMatrizDTO>();
        }

    }
}