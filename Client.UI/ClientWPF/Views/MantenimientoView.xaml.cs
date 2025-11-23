using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClientWPF.Controllers;
using Entity;

namespace ClientWPF.Views
{
    // Clase auxiliar para mostrar en la grilla
    public class DetalleVisual
    {
        public long ActivoId { get; set; }
        public string NombreActivo { get; set; }
        public string ActividadCodigo { get; set; }
        public string NombreActividad { get; set; }
        public decimal Valor { get; set; }
    }

    public partial class MantenimientoView : Window
    {
        MantenimientoController controller = new MantenimientoController();
        List<DetalleVisual> listaDetalles = new List<DetalleVisual>();

        public MantenimientoView()
        {
            InitializeComponent();
            CargarCombos();
            dpFecha.SelectedDate = DateTime.Now;
        }

        // --- CARGA INICIAL ---
        void CargarCombos()
        {
            try
            {
                // Cargar Activos
                var activos = controller.ListarActivos();
                cboActivo.ItemsSource = activos;
                cboActivo.DisplayMemberPath = "Descripcion";
                cboActivo.SelectedValuePath = "Id";
                gridActivos.ItemsSource = activos; // Llenar también la tabla de la pestaña 2

                // Cargar Actividades
                var actividades = controller.ListarActividades();
                cboActividad.ItemsSource = actividades;
                cboActividad.DisplayMemberPath = "Descripcion";
                cboActividad.SelectedValuePath = "Id";
                gridActividades.ItemsSource = actividades; // Llenar también la tabla de la pestaña 2
            }
            catch { }
        }

        // --- PESTAÑA 1: REGISTRO ---

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (cboActivo.SelectedItem == null || cboActividad.SelectedItem == null || string.IsNullOrEmpty(txtCosto.Text))
            {
                MessageBox.Show("Seleccione Activo, Actividad y Costo");
                return;
            }

            if (!decimal.TryParse(txtCosto.Text, out decimal costo))
            {
                MessageBox.Show("Costo inválido");
                return;
            }

            var activo = (ItemComboDTO)cboActivo.SelectedItem;
            var actividad = (ItemComboDTO)cboActividad.SelectedItem;

            listaDetalles.Add(new DetalleVisual
            {
                ActivoId = long.Parse(activo.Id),
                NombreActivo = activo.Descripcion,
                ActividadCodigo = actividad.Id,
                NombreActividad = actividad.Descripcion,
                Valor = costo
            });

            ActualizarGrillaDetalle();
            txtCosto.Text = "";
        }

        private void BtnQuitarDetalle_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var item = boton.DataContext as DetalleVisual;

            if (item != null)
            {
                listaDetalles.Remove(item);
                ActualizarGrillaDetalle();
            }
        }

        void ActualizarGrillaDetalle()
        {
            gridDetalle.ItemsSource = null;
            gridDetalle.ItemsSource = listaDetalles;
            lblTotal.Text = "$ " + listaDetalles.Sum(x => x.Valor).ToString("N2");
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (listaDetalles.Count == 0) { MessageBox.Show("Agregue al menos un detalle"); return; }
            if (string.IsNullOrWhiteSpace(txtResponsable.Text)) { MessageBox.Show("Ingrese Responsable"); return; }

            var request = new MantenimientoRequest
            {
                Fecha = dpFecha.SelectedDate ?? DateTime.Now,
                Responsable = txtResponsable.Text,
                Detalles = listaDetalles.Select(x => new DetalleMantDTO
                {
                    ActivoId = x.ActivoId,
                    ActividadCodigo = x.ActividadCodigo,
                    Valor = x.Valor
                }).ToList()
            };

            var resp = controller.GuardarMantenimiento(request);
            MessageBox.Show(resp.Mensaje);

            if (resp.Exito)
            {
                listaDetalles.Clear();
                ActualizarGrillaDetalle();
                txtResponsable.Text = "";
            }
        }

        // --- PESTAÑA 2: CATÁLOGOS ---

        private void BtnGuardarActividad_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodAct.Text) || string.IsNullOrEmpty(txtNomAct.Text))
            {
                MessageBox.Show("Ingrese código y nombre");
                return;
            }

            var resp = controller.GuardarActividad(txtCodAct.Text, txtNomAct.Text);
            MessageBox.Show(resp.Mensaje);

            if (resp.Exito)
            {
                txtCodAct.Text = ""; txtNomAct.Text = "";
                CargarCombos();
            }
        }

        private void BtnGuardarActivo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Función de crear activo local pendiente.");
        }

        // --- PESTAÑA 3: REPORTES ---

        private void BtnRepGastos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reporte de Gastos: Pendiente de implementación.");
        }

        private void BtnRepMatriz_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Matriz Cruzada: Pendiente de implementación.");
        }

        // --- UTILIDADES VENTANA ---
        private void BtnCerrar_Click(object sender, RoutedEventArgs e) { this.Close(); }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}