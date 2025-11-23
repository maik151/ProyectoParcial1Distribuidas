using System.Windows;
using System.Windows.Input;
using ClientWPF.Views;

namespace ClientWPF
{
    public partial class MainWindow : Window
    {
        // --- CONSTRUCTOR PRINCIPAL (El que se usa al loguearse) ---
        public MainWindow(string nombreUsuario, string rol)
        {
            InitializeComponent(); // 1. Dibuja la ventana

            // 2. EJECUTAR LÓGICA VISUAL INMEDIATAMENTE
            // Concatenar nombre y rol en el título
            txtBienvenida.Text = $"Bienvenido, {nombreUsuario}";

            // 3. VALIDAR SEGURIDAD (OCULTAR BOTÓN)
            // Limpiamos el rol por si viene con espacios o minúsculas
            string rolSeguro = rol?.Trim().ToUpper();

            if (rolSeguro != "ADMIN")
            {
                // Si NO es ADMIN, ocultamos el botón a la fuerza
                btnSeguridad.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Si ES ADMIN, lo mostramos
                btnSeguridad.Visibility = Visibility.Visible;
            }
        }

        // --- CONSTRUCTOR VACÍO (Solo para que Visual Studio no llore) ---
        public MainWindow()
        {
            InitializeComponent();
        }

        // --- ARRASTRAR VENTANA ---
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // --- CERRAR SESIÓN ---
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginView login = new LoginView();
            login.Show();
            this.Close();
        }

        // --- BOTONES DE VENTANA (NUEVOS) ---

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // --- BOTONES DE MÓDULOS ---
        private void BtnSeguridad_Click(object sender, RoutedEventArgs e)
        {
            UsuariosView view = new UsuariosView();
            view.ShowDialog();
        }
        // ✅ MÓDULO CONTABILIDAD (ACTUALIZADO)
        private void BtnContabilidad_Click(object sender, RoutedEventArgs e)
        {
            var menuContabilidad = new MenuContabilidad();
            menuContabilidad.ShowDialog();
        }

        // Placeholders para los otros botones
        private void BtnActivos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Módulo Activos - En construcción", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnMantenimiento_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Módulo Mantenimiento - En construcción", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBiblioteca_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Módulo Biblioteca - En construcción", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
