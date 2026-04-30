using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ApEn.ViewModels;

namespace ApEn.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel VM => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                VM.RequestOpenTagDialog = OpenTagDialog;
                try
                {
                    Icon = new System.Windows.Media.Imaging.BitmapImage(
                        new Uri("pack://application:,,,/Assets/apen_logo.png"));
                }
                catch { }
            };
        }

        // Abre/cierra el popup de filtros al hacer clic en el botón
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            TagFilterPopup.IsOpen = !TagFilterPopup.IsOpen;
        }

        // Cerrar popup al hacer clic fuera
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TagFilterPopup.IsOpen)
                TagFilterPopup.IsOpen = false;
        }

        private void AppNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var app = VM.SelectedApp;
            if (app is null) return;
            if (e.Key == Key.Enter)  { app.CommitEditCommand.Execute(null); e.Handled = true; }
            if (e.Key == Key.Escape) { app.CancelEditCommand.Execute(null); e.Handled = true; }
        }

        private void OpenTagDialog_Click(object sender, RoutedEventArgs e)
        {
            TagFilterPopup.IsOpen = false;
            OpenTagDialog();
        }

        private void OpenTagDialog()
        {
            var dlg = new TagDialog { Owner = this };
            if (dlg.ShowDialog() == true)
                VM.CreateTag(dlg.TagName, dlg.TagColor);
        }
    }
}
