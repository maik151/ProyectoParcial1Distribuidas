using System;
using System.Windows;
using System.Windows.Controls;
using ClientWPF.Controllers;
using Entity;

namespace ClientWPF.Views
{
    public partial class ActivosView : Window
    {
        private readonly ActivosController _controller;

        public ActivosView()
        {
            InitializeComponent();
            _controller = new ActivosController();

            CargarTipos();
            CargarActivos();
        }

        private void CargarTipos()
        {
            var resp = _controller.ListarTipos("");
            cboTipo.ItemsSource = resp.Tipos;
        }

        private void CargarActivos()
        {
            var resp = _controller.ListarActivos("");
            gridActivos.ItemsSource = resp.Activos;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long id = 0;
                long.TryParse(txtActivoId.Text, out id);

                var dto = new ActivoDTO
                {
                    ActivoId = id,
                    Nombre = txtActivoNombre.Text,
                    TipoActivoCodigo = (string?)cboTipo.SelectedValue ?? "",
                    ValorCompra = decimal.Parse(txtValorCompra.Text),
                    FechaAdquisicion = dpFechaAdquisicion.SelectedDate ?? DateTime.Now,
                    PeriodosDepreciacionTotal = short.Parse(txtPeriodos.Text)
                };

                var req = new ActivoRequest
                {
                    Comando = "A_GUARDAR",
                    Activo = dto
                };

                var resp = _controller.GuardarActivo(dto);

                MessageBox.Show(resp.Mensaje);

                CargarActivos();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtActivoId.Text))
            {
                MessageBox.Show("Seleccione un activo.");
                return;
            }

            long id = long.Parse(txtActivoId.Text);

            var resp = _controller.EliminarActivo(id);
            MessageBox.Show(resp.Mensaje);

            CargarActivos();
            Limpiar();
        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            txtActivoId.Text = "";
            txtActivoNombre.Text = "";
            cboTipo.SelectedIndex = -1;
            txtValorCompra.Text = "";
            dpFechaAdquisicion.SelectedDate = null;
            txtPeriodos.Text = "";
        }

        private void GridActivos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridActivos.SelectedItem is ActivoDTO a)
            {
                txtActivoId.Text = a.ActivoId.ToString();
                txtActivoNombre.Text = a.Nombre;
                cboTipo.SelectedValue = a.TipoActivoCodigo;
                txtValorCompra.Text = a.ValorCompra.ToString();
                dpFechaAdquisicion.SelectedDate = a.FechaAdquisicion;
                txtPeriodos.Text = a.PeriodosDepreciacionTotal.ToString();
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

