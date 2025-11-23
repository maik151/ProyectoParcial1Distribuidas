using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ClientWPF.Controllers;

namespace ClientWPF.Views
{
    public partial class EstadoResultadosView : Window
    {
        private ReporteController controller;

        public EstadoResultadosView()
        {
            InitializeComponent();
            controller = new ReporteController();

            // Establecer fechas por defecto (mes actual)
            dpFechaInicio.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpFechaFin.SelectedDate = DateTime.Now;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (dpFechaInicio.SelectedDate == null || dpFechaFin.SelectedDate == null)
            {
                MessageBox.Show("Seleccione ambas fechas", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpFechaInicio.SelectedDate > dpFechaFin.SelectedDate)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor a la fecha fin", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var respuesta = controller.ObtenerEstadoResultados(dpFechaInicio.SelectedDate.Value, dpFechaFin.SelectedDate.Value);

            if (respuesta != null && respuesta.Exito)
            {
                gridEstadoResultados.ItemsSource = null;
                gridEstadoResultados.ItemsSource = respuesta.Lineas;

                // Mostrar utilidad
                txtUtilidad.Text = respuesta.Utilidad.ToString("C2");

                // Cambiar color según utilidad positiva o negativa
                if (respuesta.Utilidad >= 0)
                {
                    txtUtilidad.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#51CF66"));
                }
                else
                {
                    txtUtilidad.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B6B"));
                }

                if (respuesta.Lineas.Count == 0)
                {
                    MessageBox.Show("No hay datos para el rango de fechas seleccionado", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(respuesta?.Mensaje ?? "Error al generar reporte", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
