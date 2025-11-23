using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data; // NECESARIO PARA LA MATRIZ
using ClientWPF.Controllers;
using Entity;

namespace ClientWPF.Views
{
    // Clase auxiliar para la grilla
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
        List<ItemComboDTO> listaActividadesFull = new List<ItemComboDTO>();
        List<ItemComboDTO> listaActivosFull = new List<ItemComboDTO>();

        public MantenimientoView()
        {
            InitializeComponent();
            CargarDatosIniciales();

            if (dpFecha != null) dpFecha.SelectedDate = DateTime.Now;
        }

        // --- CARGA DE DATOS ---
        void CargarDatosIniciales()
        {
            try
            {
                // 1. Activos
                listaActivosFull = controller.ListarActivos();
                if (cboActivo != null)
                {
                    cboActivo.ItemsSource = listaActivosFull;
                    cboActivo.DisplayMemberPath = "Descripcion";
                    cboActivo.SelectedValuePath = "Id";
                }
                if (gridActivos != null) gridActivos.ItemsSource = listaActivosFull;

                // 2. Actividades
                listaActividadesFull = controller.ListarActividades();
                if (cboActividad != null)
                {
                    cboActividad.ItemsSource = listaActividadesFull;
                    cboActividad.DisplayMemberPath = "Descripcion";
                    cboActividad.SelectedValuePath = "Id";
                }
                if (gridActividades != null) gridActividades.ItemsSource = listaActividadesFull;
            }
            catch { }
        }

        // =================================================
        // PESTAÑA 1: REGISTRO
        // =================================================

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
            if (gridDetalle != null)
            {
                gridDetalle.ItemsSource = null;
                gridDetalle.ItemsSource = listaDetalles;
            }
            if (lblTotal != null)
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

        // =================================================
        // PESTAÑA 2: CATÁLOGOS
        // =================================================

        // --- ACTIVIDADES ---
        private void BtnGuardarActividad_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodAct.Text) || string.IsNullOrEmpty(txtNomAct.Text))
            {
                MessageBox.Show("Ingrese datos"); return;
            }
            var resp = controller.GuardarActividad(txtCodAct.Text, txtNomAct.Text);
            MessageBox.Show(resp.Mensaje);
            if (resp.Exito) { BtnNuevaActividad_Click(null, null); CargarDatosIniciales(); }
        }

        private void BtnNuevaActividad_Click(object sender, RoutedEventArgs e)
        {
            txtCodAct.Text = ""; txtNomAct.Text = ""; txtCodAct.IsEnabled = true;
        }

        private void BtnEditarActividad_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.DataContext is ItemComboDTO item)
            {
                txtCodAct.Text = item.Id;
                txtNomAct.Text = item.Descripcion;
                txtCodAct.IsEnabled = false;
            }
        }

        private void BtnEliminarActividad_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.DataContext is ItemComboDTO item)
            {
                if (MessageBox.Show($"¿Eliminar '{item.Descripcion}'?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var resp = controller.EliminarActividad(item.Id);
                    MessageBox.Show(resp.Mensaje);
                    if (resp.Exito) CargarDatosIniciales();
                }
            }
        }

        private void TxtBuscarActividad_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarActividad.Text.ToLower();
            gridActividades.ItemsSource = listaActividadesFull
                .Where(x => x.Descripcion.ToLower().Contains(filtro) || x.Id.ToLower().Contains(filtro)).ToList();
        }

        // --- ACTIVOS ---
        private void BtnGuardarActivo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdActivo.Text) || string.IsNullOrEmpty(txtNomActivo.Text)) { MessageBox.Show("Ingrese datos"); return; }
            if (long.TryParse(txtIdActivo.Text, out long id))
            {
                var resp = controller.GuardarActivo(id, txtNomActivo.Text);
                MessageBox.Show(resp.Mensaje);
                if (resp.Exito) { BtnNuevoActivo_Click(null, null); CargarDatosIniciales(); }
            }
            else MessageBox.Show("ID numérico");
        }

        private void BtnNuevoActivo_Click(object sender, RoutedEventArgs e)
        {
            txtIdActivo.Text = ""; txtNomActivo.Text = ""; txtIdActivo.IsEnabled = true;
        }

        private void BtnEditarActivo_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.DataContext is ItemComboDTO item)
            {
                txtIdActivo.Text = item.Id;
                txtNomActivo.Text = item.Descripcion;
                txtIdActivo.IsEnabled = false;
            }
        }

        private void BtnEliminarActivo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pendiente en Servidor");
        }

        private void TxtBuscarActivo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarActivo.Text.ToLower();
            gridActivos.ItemsSource = listaActivosFull
                .Where(x => x.Descripcion.ToLower().Contains(filtro) || x.Id.ToLower().Contains(filtro)).ToList();
        }

        // =================================================
        // PESTAÑA 3: REPORTES
        // =================================================

        private void BtnRepGastos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var datos = controller.ObtenerReporteGastos();
                gridReportes.ItemsSource = null;
                gridReportes.Columns.Clear();
                gridReportes.AutoGenerateColumns = true;
                gridReportes.ItemsSource = datos;
                if (datos.Count == 0) MessageBox.Show("No hay datos.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnRepMatriz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var datosPlanos = controller.ObtenerReporteMatriz();
                if (datosPlanos.Count == 0) { MessageBox.Show("No hay datos."); return; }

                DataTable dt = ConstruirMatrizDinamica(datosPlanos);

                gridReportes.ItemsSource = null;
                gridReportes.Columns.Clear();
                gridReportes.AutoGenerateColumns = true;
                gridReportes.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private DataTable ConstruirMatrizDinamica(List<ReporteMatrizDTO> datos)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ACTIVO", typeof(string));

            var actividades = datos.Select(x => x.Actividad).Distinct().OrderBy(x => x).ToList();
            foreach (var act in actividades) dt.Columns.Add(act, typeof(decimal));

            dt.Columns.Add("TOTAL", typeof(decimal));

            var activos = datos.Select(x => x.Activo).Distinct().OrderBy(x => x).ToList();
            foreach (var nomActivo in activos)
            {
                DataRow row = dt.NewRow();
                row["ACTIVO"] = nomActivo;
                decimal total = 0;
                foreach (var act in actividades)
                {
                    var reg = datos.FirstOrDefault(x => x.Activo == nomActivo && x.Actividad == act);
                    decimal val = reg != null ? reg.Valor : 0;
                    row[act] = val;
                    total += val;
                }
                row["TOTAL"] = total;
                dt.Rows.Add(row);
            }
            return dt;
        }

        // --- UTILIDADES ---
        private void BtnCerrar_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void BtnRecargar_Click(object sender, RoutedEventArgs e) { CargarDatosIniciales(); }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
    }
}