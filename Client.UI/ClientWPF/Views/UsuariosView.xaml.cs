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
            txtRol.Text = "";
            txtUser.Focus();
        }

        // --- BOTÓN GUARDAR (INSERTA O MODIFICA SEGÚN ID) ---
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Complete los datos obligatorios.");
                return;
            }

            if (!int.TryParse(txtRol.Text, out int rolId))
            {
                MessageBox.Show("El Rol ID debe ser un número (1 o 2).");
                return;
            }

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

        // --- BOTÓN EDITAR (EN LA FILA) ---
        private void BtnEditarFila_Click(object sender, RoutedEventArgs e)
        {
            // Truco para obtener el dato de la fila donde se hizo clic
            var boton = sender as Button;
            var usuarioFila = boton.DataContext as UsuarioDTO;

            if (usuarioFila != null)
            {
                // Llenamos el formulario con los datos de esa fila
                idSeleccionado = usuarioFila.UsuarioId;
                txtUser.Text = usuarioFila.NombreUsuario;
                txtPass.Text = usuarioFila.Clave; // Nota: En prod no se debería mostrar la clave
                txtNombre.Text = usuarioFila.NombreCompleto;
                txtRol.Text = usuarioFila.RolId.ToString();
            }
        }

        // --- BOTÓN ELIMINAR (EN LA FILA) ---
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

        // --- BOTÓN RECARGAR (HEADER) ---
        private void BtnRecargar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        // --- BOTÓN CERRAR ---
        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // --- ARRASTRAR VENTANA ---
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}