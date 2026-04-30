using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ApEn.Converters
{
    public class TabVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => (int)v == int.Parse(p?.ToString() ?? "0") ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class TabWeightConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => (int)v == int.Parse(p?.ToString() ?? "0") ? FontWeights.SemiBold : FontWeights.Normal;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class TabColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush A = new(Color.FromRgb(0x6C, 0x63, 0xFF));
        private static readonly SolidColorBrush I = new(Color.FromRgb(0x8A, 0x8F, 0xA8));
        public object Convert(object v, Type t, object p, CultureInfo c)
            => (int)v == int.Parse(p?.ToString() ?? "0") ? A : I;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => v is int n && n == 0 ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class NonZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => v is int n && n > 0 ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => v is true ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => v is false ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            var parts = p?.ToString()?.Split('|') ?? Array.Empty<string>();
            return v is true
                ? (parts.Length > 0 ? parts[0] : "")
                : (parts.Length > 1 ? parts[1] : "");
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            try
            {
                var col = (Color)(ColorConverter.ConvertFromString(v?.ToString() ?? "#FF6C63FF") ?? Colors.Purple);
                return new SolidColorBrush(col);
            }
            catch { return new SolidColorBrush(Colors.Purple); }
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    /// <summary>Convierte bool a GridLength (para colapsar columnas sin animación).</summary>
    public class BoolToGridLengthConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            if (v is not true) return new GridLength(0);
            double w = double.TryParse(p?.ToString(), out var n) ? n : 220;
            return new GridLength(w);
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }

    /// <summary>
    /// MultiValueConverter: [0]=ObservableCollection<string>, [1]=string tagName
    /// Devuelve true si la coleccion contiene el valor — para resaltar pill activa.
    /// </summary>
    public class CollectionContainsBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type t, object p, CultureInfo c)
        {
            if (values.Length < 2) return false;
            if (values[0] is System.Collections.IEnumerable col && values[1] is string tag)
                return col.Cast<string>().Contains(tag);
            return false;
        }
        public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
            => throw new NotImplementedException();
    }
}
