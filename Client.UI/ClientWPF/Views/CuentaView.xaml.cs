using System;
using System.Windows;
using System.Windows.Input;
using ClientWPF.Controllers;
using Entity;

namespace ClientWPF.Views
{
    public partial class CuentaView : Window
    {
        private CuentaController cuentaController;
        private TipoCuentaController tipoCuentaController;
        private long idActual = 0;

        public CuentaView()
        {
            InitializeComponent();
            cuentaController = new CuentaController();
            tipoCuentaController = new TipoCuentaController();
            CargarTiposCuenta();
            CargarDatos();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CargarTiposCuenta()
        {
            var listaTipos = tipoCuentaController.Listar();
            cboTipoCuenta.ItemsSource = listaTipos;
        }

        private void CargarDatos()
        {
            var lista = cuentaController.Listar();
            gridCuentas.ItemsSource = null;
            gridCuentas.ItemsSource = lista;
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            idActual = 0;
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtNivel.Text = "";
            cboTipoCuenta.SelectedIndex = -1;
            chkEsCuentaMovimiento.IsChecked = true;  // ✅ CAMBIO: true por defecto
            txtCodigo.Focus();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                cboTipoCuenta.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtNivel.Text))
            {
                MessageBox.Show("Complete todos los campos", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtNivel.Text, out int nivel))
            {
                MessageBox.Show("El nivel debe ser un número entero", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cuenta = new CuentaDTO
            {
                CuentaId = idActual,
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                TipoCuentaId = Convert.ToInt32(cboTipoCuenta.SelectedValue),
                Nivel = nivel,
                EsCuentaMovimiento = true  // ✅ CAMBIO: SIEMPRE TRUE
            };

            var respuesta = cuentaController.Guardar(cuenta);

            if (respuesta != null && respuesta.Exito)
            {
                MessageBox.Show(respuesta.Mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnNuevo_Click(null, null);
                CargarDatos();
            }
            else
            {
                MessageBox.Show(respuesta?.Mensaje ?? "Error desconocido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (gridCuentas.SelectedItem is CuentaDTO c)
            {
                idActual = c.CuentaId;
                txtCodigo.Text = c.Codigo;
                txtNombre.Text = c.Nombre;
                txtNivel.Text = c.Nivel.ToString();
                cboTipoCuenta.SelectedValue = c.TipoCuentaId;
                chkEsCuentaMovimiento.IsChecked = c.EsCuentaMovimiento;
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (gridCuentas.SelectedItem is CuentaDTO c)
            {
                var result = MessageBox.Show($"¿Eliminar {c.Nombre}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var respuesta = cuentaController.Eliminar(c.CuentaId);
                    if (respuesta != null && respuesta.Exito)
                    {
                        MessageBox.Show(respuesta.Mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show(respuesta?.Mensaje ?? "Error desconocido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnRecargar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}