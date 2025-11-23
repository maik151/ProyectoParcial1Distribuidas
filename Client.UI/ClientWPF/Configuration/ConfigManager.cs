using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace ClientWPF.Configuration
{
    public static class ConfigManager
    {
        private static IConfiguration _config;

        static ConfigManager()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _config = builder.Build();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR CRÍTICO CONFIG: {ex.Message}");
                throw;
            }
        }

        // --- SEGURIDAD ---
        public static string IpSeguridad => _config["Endpoints:IpSeguridad"]!;
        public static int PuertoSeguridad => int.Parse(_config["Endpoints:PuertoSeguridad"]!);

        // --- MANTENIMIENTO (NUEVO) ---
        public static string IpMantenimiento
        {
            get
            {
                try { return _config["Endpoints:IpMantenimiento"]!; }
                catch { return "127.0.0.1"; }
            }
        }
        public static int PuertoMantenimiento
        {
            get
            {
                try { return int.Parse(_config["Endpoints:PuertoMantenimiento"]!); }
                catch { return 9001; }
            }
        }
        // ========== CONTABILIDAD ========== ✅ NUEVO
        public static string IpContabilidad
        {
            get
            {
                try { return _config["Endpoints:IpContabilidad"]; }
                catch { MessageBox.Show("No se encuentra 'IpContabilidad'"); return "127.0.0.1"; }
            }
        }

        public static int PuertoContabilidad
        {
            get
            {
                try { return int.Parse(_config["Endpoints:PuertoContabilidad"]); }
                catch { MessageBox.Show("No se encuentra 'PuertoContabilidad'"); return 8002; }
            }
        }
    }
}