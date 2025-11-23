using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ClientWPF.Controllers;
using Entity;

namespace ClientWPF.Views
{
    public partial class ComprobanteView : Window
    {
        private ComprobanteController comprobanteController;
        private CuentaController cuentaController;
        private long comprobanteIdActual = 0;
        private List<DetalleComprobanteDTO> listaDetalle;

        public ComprobanteView()
        {
            InitializeComponent();
            comprobanteController = new ComprobanteController();
            cuentaController = new CuentaController();
            listaDetalle = new List<DetalleComprobanteDTO>();

            CargarCuentas();
            CargarComprobantes();
            dpFecha.SelectedDate = DateTime.Now;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CargarCuentas()
        {
            var listaCuentas = cuentaController.Listar();
            // Filtrar solo cuentas de movimiento
            var cuentasMovimiento = listaCuentas.Where(c => c.EsCuentaMovimiento).ToList();
            cboCuenta.ItemsSource = cuentasMovimiento;
        }

        private void CargarComprobantes()
        {
            var lista = comprobanteController.Listar();
            gridComprobantes.ItemsSource = null;
            gridComprobantes.ItemsSource = lista;
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            comprobanteIdActual = 0;
            txtNumero.Text = "";
            dpFecha.SelectedDate = DateTime.Now;
            txtObservaciones.Text = "";

            listaDetalle.Clear();
            gridDetalle.ItemsSource = null;

            cboCuenta.SelectedIndex = -1;
            txtDebe.Text = "0";
            txtHaber.Text = "0";

            ActualizarTotales();
            txtNumero.Focus();
        }

        private void BtnAgregarDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (cboCuenta.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una cuenta", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtDebe.Text, out decimal debe))
                debe = 0;

            if (!decimal.TryParse(txtHaber.Text, out decimal haber))
                haber = 0;

            if (debe == 0 && haber == 0)
            {
                MessageBox.Show("Debe ingresar un valor en Debe o Haber", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cuentaSeleccionada = cboCuenta.SelectedItem as CuentaDTO;

            var detalle = new DetalleComprobanteDTO
            {
                CuentaId = Convert.ToInt64(cboCuenta.SelectedValue),
                CodigoCuenta = cuentaSeleccionada.Codigo,
                NombreCuenta = cuentaSeleccionada.Nombre,
                CantidadDebe = debe,
                CantidadHaber = haber
            };

            listaDetalle.Add(detalle);
            gridDetalle.ItemsSource = null;
            gridDetalle.ItemsSource = listaDetalle;

            // Limpiar controles
            cboCuenta.SelectedIndex = -1;
            txtDebe.Text = "0";
            txtHaber.Text = "0";

            ActualizarTotales();
        }

        private void BtnEliminarDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (gridDetalle.SelectedItem is DetalleComprobanteDTO detalle)
            {
                listaDetalle.Remove(detalle);
                gridDetalle.ItemsSource = null;
                gridDetalle.ItemsSource = listaDetalle;
                ActualizarTotales();
            }
        }

        private void ActualizarTotales()
        {
            decimal totalDebe = listaDetalle.Sum(d => d.CantidadDebe);
            decimal totalHaber = listaDetalle.Sum(d => d.CantidadHaber);

            txtTotalDebe.Text = totalDebe.ToString("N2");
            txtTotalHaber.Text = totalHaber.ToString("N2");

            if (totalDebe == totalHaber && totalDebe > 0)
            {
                txtEstadoCuadre.Text = "✓ Cuadrado";
                txtEstadoCuadre.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#51CF66"));
            }
            else
            {
                txtEstadoCuadre.Text = "⚠ Descuadrado";
                txtEstadoCuadre.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B6B"));
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text))
            {
                MessageBox.Show("Ingrese el número de comprobante", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpFecha.SelectedDate == null)
            {
                MessageBox.Show("Seleccione la fecha", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (listaDetalle.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un detalle", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal totalDebe = listaDetalle.Sum(d => d.CantidadDebe);
            decimal totalHaber = listaDetalle.Sum(d => d.CantidadHaber);

            if (totalDebe != totalHaber)
            {
                MessageBox.Show("El comprobante debe estar cuadrado (Debe = Haber)", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var comprobante = new ComprobanteContableDTO
            {
                ComprobanteId = comprobanteIdActual,
                Numero = txtNumero.Text,
                Fecha = dpFecha.SelectedDate.Value,
                Observaciones = txtObservaciones.Text,
                ModuloOrigen = "CONTABILIDAD",
                ReferenciaOrigenId = null,
                Detalles = new List<DetalleComprobanteDTO>(listaDetalle)
            };

            var respuesta = comprobanteController.Guardar(comprobante);

            if (respuesta != null && respuesta.Exito)
            {
                MessageBox.Show(respuesta.Mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnNuevo_Click(null, null);
                CargarComprobantes();
            }
            else
            {
                MessageBox.Show(respuesta?.Mensaje ?? "Error desconocido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GridComprobantes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (gridComprobantes.SelectedItem is ComprobanteContableDTO comp)
            {
                comprobanteIdActual = comp.ComprobanteId;
                txtNumero.Text = comp.Numero;
                dpFecha.SelectedDate = comp.Fecha;
                txtObservaciones.Text = comp.Observaciones;

                listaDetalle = new List<DetalleComprobanteDTO>(comp.Detalles);
                gridDetalle.ItemsSource = null;
                gridDetalle.ItemsSource = listaDetalle;

                ActualizarTotales();
            }
        }

        private void BtnRecargar_Click(object sender, RoutedEventArgs e)
        {
            CargarComprobantes();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}