using System.Collections.Generic;
using ClientWPF.ConectionSockets;
using ClientWPF.Configuration;
using Entity;

namespace ClientWPF.Controllers
{
    public class MantenimientoController
    {
        ConexionSockets clienteSocket = new ConexionSockets();

        // --- 1. LISTAR (Para los Combos y Tablas) ---
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

        // --- 2. GUARDAR CATÁLOGOS (Lo que te faltaba) ---
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

        // (Opcional: Si no tienes Guardar Activo en el server, devuelve falso por ahora)
        public RespuestaBase GuardarActivo(long id, string nombre)
        {
            // Simulación hasta que implementes esto en el Backend si lo necesitas
            return new RespuestaBase { Exito = false, Mensaje = "Guardar Activo Local en construcción" };
        }

        // --- 3. GUARDAR MANTENIMIENTO (PRINCIPAL) ---
        public RespuestaBase GuardarMantenimiento(MantenimientoRequest transaccion)
        {
            transaccion.Comando = "GUARDAR_MANTENIMIENTO";
            return clienteSocket.Enviar<RespuestaBase>(
                ConfigManager.IpMantenimiento, ConfigManager.PuertoMantenimiento, transaccion);
        }

        // NOTA: He quitado los métodos de Reportes para que no te den error.
    }
}