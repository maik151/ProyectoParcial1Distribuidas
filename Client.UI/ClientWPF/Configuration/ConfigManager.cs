using System;
using System.IO;
using System.Windows; // Necesario para MessageBox
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
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Ruta más segura para WPF
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _config = builder.Build();
            }
            catch (Exception ex)
            {
                // ESTO TE DIRÁ SI EL ARCHIVO NO EXISTE O ESTÁ MAL FORMADO
                MessageBox.Show($"ERROR CRÍTICO LEYENDO CONFIGURACIÓN:\n{ex.Message}", "Error Config");
                throw; // Re-lanzar para cerrar, pero ya viste el mensaje
            }
        }

        public static string IpSeguridad
        {
            get
            {
                try { return _config["Endpoints:IpSeguridad"]; }
                catch { MessageBox.Show("No se encuentra la clave 'IpSeguridad' en el JSON"); return "127.0.0.1"; }
            }
        }

        public static int PuertoSeguridad
        {
            get
            {
                try { return int.Parse(_config["Endpoints:PuertoSeguridad"]); }
                catch { MessageBox.Show("No se encuentra o no es número 'PuertoSeguridad'"); return 8000; }
            }
        }
    }
}