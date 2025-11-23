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
        // --- 1. LISTAR ---
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

        // --- 2. GUARDAR CATÁLOGOS ---
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

        // --- 3. ELIMINAR ---
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

        // --- 4. GUARDAR MANTENIMIENTO ---
        public RespuestaBase GuardarMantenimiento(MantenimientoRequest transaccion)
        {
            transaccion.Comando = "GUARDAR_MANTENIMIENTO";
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, transaccion);
        }

        // --- 5. REPORTES (ESTOS SON LOS QUE FALTABAN) ---
        public List<ReporteGastoDTO> ObtenerReporteGastos(DateTime inicio, DateTime fin)
        {
            
            var req = new ReporteRequest
            {
                Comando = "REPORTE_GASTOS",
                FechaInicio = inicio,
                FechaFin = fin
            };

            
            var resp = clienteSocket.Enviar<ReporteResponse>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);

            return resp?.ReporteGastos ?? new List<ReporteGastoDTO>();
        }

        public List<ReporteMatrizDTO> ObtenerReporteMatriz(DateTime inicio, DateTime fin)
        {
            var req = new ReporteRequest
            {
                Comando = "REPORTE_MATRIZ",
                FechaInicio = inicio,
                FechaFin = fin
            };

            var resp = clienteSocket.Enviar<ReporteResponse>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, req);

            return resp?.ReporteMatriz ?? new List<ReporteMatrizDTO>();
        }
    }
}