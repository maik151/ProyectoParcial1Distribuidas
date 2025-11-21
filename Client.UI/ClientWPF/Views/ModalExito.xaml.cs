using System.Windows;
using System.Windows.Input;

namespace ClientWPF.Views
{
    public partial class ModalExito : Window
    {
        // Constructor que recibe el mensaje
        public ModalExito(string mensaje)
        {
            InitializeComponent();
            txtMensaje.Text = mensaje;
        }

        // Botón Continuar: Cierra la ventana retornando True
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        // Permitir arrastrar la ventana (ya que no tiene bordes)
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}