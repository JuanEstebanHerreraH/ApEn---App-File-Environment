using System.Windows;

namespace ApEn.Services
{
    /// <summary>
    /// Cambia el tema de la app intercambiando el primer MergedDictionary de App.Resources.
    /// Los .xaml de tema están embebidos como Resource en el ensamblado (pack URI).
    /// </summary>
    public static class ThemeService
    {
        private static bool _isDark = true;
        public static bool IsDark => _isDark;

        public static void Apply(bool isDark)
        {
            _isDark = isDark;
            var uri = isDark
                ? new Uri("pack://application:,,,/Themes/DarkTheme.xaml",  UriKind.Absolute)
                : new Uri("pack://application:,,,/Themes/LightTheme.xaml", UriKind.Absolute);

            var dict = new ResourceDictionary { Source = uri };
            Application.Current.Resources.MergedDictionaries[0] = dict;
        }

        public static void Toggle() => Apply(!_isDark);
    }
}
