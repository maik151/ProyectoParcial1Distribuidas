using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClientWPF.Controllers;
using Entity; // Para ItemComboDTO

namespace ClientWPF.Views
{
    // Clase auxiliar para mostrar en la grilla de detalles
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

        // Listas completas para permitir filtrado en el cliente
        private List<ItemComboDTO> listaActividadesFull = new List<ItemComboDTO>();
        private List<ItemComboDTO> listaActivosFull = new List<ItemComboDTO>();

        public MantenimientoView()
        {
            InitializeComponent();
            CargarCombos(); // Carga inicial de datos
            dpFecha.SelectedDate = DateTime.Now;
        }

        // --- CARGA DE DATOS (ACTIVOS Y ACTIVIDADES) ---
        void CargarCombos()
        {
            try
            {
                // 1. Activos
                listaActivosFull = controller.ListarActivos();

                cboActivo.ItemsSource = listaActivosFull;
                cboActivo.DisplayMemberPath = "Descripcion";
                cboActivo.SelectedValuePath = "Id";

                gridActivos.ItemsSource = listaActivosFull; // Llenar tabla pestaña 2

                // 2. Actividades
                listaActividadesFull = controller.ListarActividades();

                cboActividad.ItemsSource = listaActividadesFull;
                cboActividad.DisplayMemberPath = "Descripcion";
                cboActividad.SelectedValuePath = "Id";

                gridActividades.ItemsSource = listaActividadesFull; // Llenar tabla pestaña 2
            }
            catch { }
        }

        // =================================================
        // PESTAÑA 1: REGISTRO (TRANSACCIÓN)
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

        // =================================================
        // PESTAÑA 2: CATÁLOGOS (CRUD COMPLETO)
        // =================================================

        // --- ACTIVIDADES ---

        private void BtnNuevaActividad_Click(object sender, RoutedEventArgs e)
        {
            txtCodAct.Text = "";
            txtNomAct.Text = "";
            txtCodAct.IsEnabled = true; // Permitir editar código
            txtCodAct.Focus();
        }

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
                BtnNuevaActividad_Click(null, null); // Limpiar
                CargarCombos(); // Recargar tabla y combo
            }
        }

        private void BtnEditarActividad_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            // Obtenemos el objeto de la fila
            if (boton.DataContext is ItemComboDTO item)
            {
                txtCodAct.Text = item.Id;
                txtNomAct.Text = item.Descripcion;
                txtCodAct.IsEnabled = false; // Bloquear PK al editar
            }
        }

        private void BtnEliminarActividad_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            if (boton.DataContext is ItemComboDTO item)
            {
                if (MessageBox.Show($"¿Eliminar actividad '{item.Descripcion}'?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var resp = controller.EliminarActividad(item.Id);
                    MessageBox.Show(resp.Mensaje);

                    if (resp.Exito) CargarCombos();
                }
            }
        }

        private void TxtBuscarActividad_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filtrado local en la lista en memoria
            string filtro = txtBuscarActividad.Text.ToLower();
            gridActividades.ItemsSource = listaActividadesFull
                .Where(x => x.Descripcion.ToLower().Contains(filtro) || x.Id.ToLower().Contains(filtro))
                .ToList();
        }

        // --- ACTIVOS (Simplificado) ---

        private void BtnNuevoActivo_Click(object sender, RoutedEventArgs e)
        {
            txtIdActivo.Text = ""; txtNomActivo.Text = ""; txtIdActivo.IsEnabled = true;
        }

        private void BtnGuardarActivo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Función de crear activo local pendiente.");
        }

        private void BtnEditarActivo_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            if (boton.DataContext is ItemComboDTO item)
            {
                txtIdActivo.Text = item.Id;
                txtNomActivo.Text = item.Descripcion;
                txtIdActivo.IsEnabled = false;
            }
        }

        private void BtnEliminarActivo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar Activo pendiente en Servidor");
        }

        private void TxtBuscarActivo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarActivo.Text.ToLower();
            gridActivos.ItemsSource = listaActivosFull
                .Where(x => x.Descripcion.ToLower().Contains(filtro) || x.Id.ToLower().Contains(filtro))
                .ToList();
        }


        // =================================================
        // PESTAÑA 3: REPORTES
        // =================================================

        private void BtnRepGastos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reporte de Gastos: Pendiente de implementación.");
        }

        private void BtnRepMatriz_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Matriz Cruzada: Pendiente de implementación.");
        }

        // =================================================
        // UTILIDADES VENTANA
        // =================================================
        private void BtnCerrar_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void BtnRecargar_Click(object sender, RoutedEventArgs e) { CargarCombos(); }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}