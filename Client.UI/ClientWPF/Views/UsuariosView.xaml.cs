using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClientWPF.Controllers;
using Entity; // Para UsuarioDTO

namespace ClientWPF.Views
{
    public partial class UsuariosView : Window
    {
        UsuariosController controller = new UsuariosController();
        long idSeleccionado = 0; // 0 = Nuevo, >0 = Modificar

        public UsuariosView()
        {
            InitializeComponent();
            CargarDatos();
        }

        // --- CARGAR DATOS (RELOAD) ---
        void CargarDatos()
        {
            try
            {
                var lista = controller.Listar();
                if (lista != null) gridUsuarios.ItemsSource = lista;
                else gridUsuarios.ItemsSource = null;
            }
            catch
            {
                // Manejo silencioso o log
            }
        }

        // --- LIMPIAR FORMULARIO (NUEVO) ---
        void Limpiar()
        {
            idSeleccionado = 0;
            txtUser.Text = "";
            txtPass.Text = "";
            txtNombre.Text = "";

            // Reseteamos el Combo al valor por defecto (Usuario Común)
            cboRol.SelectedIndex = 0;

            txtUser.Focus();
        }

        // --- BOTÓN GUARDAR (CON LÓGICA DE COMBOBOX) ---
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Complete los datos obligatorios.");
                return;
            }

            // OBTENER ROL DEL COMBOBOX
            // Usamos la propiedad 'Tag' que definimos en el XAML (1 o 2)
            ComboBoxItem itemSeleccionado = (ComboBoxItem)cboRol.SelectedItem;
            int rolId = int.Parse(itemSeleccionado.Tag.ToString());

            var usuario = new UsuarioDTO
            {
                UsuarioId = idSeleccionado,
                NombreUsuario = txtUser.Text,
                Clave = txtPass.Text,
                NombreCompleto = txtNombre.Text,
                RolId = rolId
            };

            var resp = controller.Guardar(usuario);
            MessageBox.Show(resp.Mensaje);

            if (resp.Exito)
            {
                CargarDatos();
                Limpiar();
            }
        }

        // --- BOTÓN NUEVO ---
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        // --- SELECCIONAR FILA PARA EDITAR ---
        private void BtnEditarFila_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var usuarioFila = boton.DataContext as UsuarioDTO;

            if (usuarioFila != null)
            {
                // Llenamos el formulario
                idSeleccionado = usuarioFila.UsuarioId;
                txtUser.Text = usuarioFila.NombreUsuario;
                txtPass.Text = usuarioFila.Clave;
                txtNombre.Text = usuarioFila.NombreCompleto;

                // SELECCIONAR EL ROL CORRECTO EN EL COMBO
                foreach (ComboBoxItem item in cboRol.Items)
                {
                    if (item.Tag.ToString() == usuarioFila.RolId.ToString())
                    {
                        cboRol.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // --- BOTÓN ELIMINAR ---
        private void BtnEliminarFila_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var usuarioFila = boton.DataContext as UsuarioDTO;

            if (usuarioFila != null)
            {
                var confirm = MessageBox.Show($"¿Estás seguro de eliminar a {usuarioFila.NombreUsuario}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    var resp = controller.Eliminar(usuarioFila.UsuarioId);
                    MessageBox.Show(resp.Mensaje);
                    if (resp.Exito) CargarDatos();
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}