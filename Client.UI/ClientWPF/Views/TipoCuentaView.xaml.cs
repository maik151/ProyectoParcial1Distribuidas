using System;
using System.Windows;
using System.Windows.Input;
using ClientWPF.Controllers;
using Entity;

namespace ClientWPF.Views
{
    public partial class TipoCuentaView : Window
    {
        private TipoCuentaController controller;
        private int idActual = 0;

        public TipoCuentaView()
        {
            InitializeComponent();
            controller = new TipoCuentaController();
            CargarDatos();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CargarDatos()
        {
            var lista = controller.Listar();
            gridTiposCuenta.ItemsSource = null;
            gridTiposCuenta.ItemsSource = lista;
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            idActual = 0;
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtCodigo.Focus();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Complete todos los campos", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tipoCuenta = new TipoCuentaDTO
            {
                TipoCuentaId = idActual,
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text
            };

            var respuesta = controller.Guardar(tipoCuenta);

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
            if (gridTiposCuenta.SelectedItem is TipoCuentaDTO tc)
            {
                idActual = tc.TipoCuentaId;
                txtCodigo.Text = tc.Codigo;
                txtNombre.Text = tc.Nombre;
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (gridTiposCuenta.SelectedItem is TipoCuentaDTO tc)
            {
                var result = MessageBox.Show($"¿Eliminar {tc.Nombre}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var respuesta = controller.Eliminar(tc.TipoCuentaId);
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