using System;
using System.Windows;
using System.Windows.Input;
using ClientWPF.Controllers;

namespace ClientWPF.Views
{
    public partial class BalanceGeneralView : Window
    {
        private ReporteController controller;

        public BalanceGeneralView()
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

            var respuesta = controller.ObtenerBalanceGeneral(dpFechaInicio.SelectedDate.Value, dpFechaFin.SelectedDate.Value);

            if (respuesta != null && respuesta.Exito)
            {
                gridBalance.ItemsSource = null;
                gridBalance.ItemsSource = respuesta.Lineas;

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