using System.Windows;
using System.Windows.Threading;

namespace ApEn
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Capturar cualquier excepción no manejada y mostrarla
            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show(
                    $"Error al iniciar ApEn:\n\n{ex.Exception.GetType().Name}\n{ex.Exception.Message}\n\n{ex.Exception.StackTrace}",
                    "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
                Shutdown(1);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                MessageBox.Show(
                    $"Error crítico:\n\n{ex.ExceptionObject}",
                    "Error crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            base.OnStartup(e);
        }
    }
}
