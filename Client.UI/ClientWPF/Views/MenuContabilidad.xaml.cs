using System.Windows;
using System.Windows.Input;

namespace ClientWPF.Views
{
    public partial class MenuContabilidad : Window
    {
        public MenuContabilidad()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnTipoCuenta_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new TipoCuentaView();
            ventana.ShowDialog();
        }

        private void BtnCuenta_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new CuentaView();
            ventana.ShowDialog();
        }

        private void BtnComprobante_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new ComprobanteView();
            ventana.ShowDialog();
        }

        // ✅ REPORTES
        private void BtnBalanceGeneral_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new BalanceGeneralView();
            ventana.ShowDialog();
        }

        private void BtnEstadoResultados_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new EstadoResultadosView();
            ventana.ShowDialog();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}