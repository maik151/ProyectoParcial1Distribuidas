using System.Windows;
using System.Windows.Input;
using ClientWPF.Controllers;

namespace ClientWPF.Views
{
    public partial class LoginView : Window
    {
        private LoginController controller;

        public LoginView()
        {
            InitializeComponent();
            controller = new LoginController();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            string pass = txtPass.Password;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Ingrese usuario y contraseña");
                return;
            }

            // 1. LLAMADA AL SERVIDOR
            var respuesta = controller.Login(user, pass);

            if (respuesta != null && respuesta.Exito)
            {
                string mensajeExito = $"Bienvenido al Sistema\nUsuario: {respuesta.NombreUsuario}\nRol: {respuesta.Rol}";

                ModalExito modal = new ModalExito(mensajeExito);
                bool? resultado = modal.ShowDialog();

                if (resultado == true)
                {
                   
                    MainWindow menu = new MainWindow(respuesta.NombreUsuario, respuesta.Rol);

                   
                    menu.Show();
                    this.Close();
                }
            }
            else
            {
                string msg = respuesta?.Mensaje ?? "Error al conectar con el servidor";
                MessageBox.Show(msg, "Error de Login");
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}