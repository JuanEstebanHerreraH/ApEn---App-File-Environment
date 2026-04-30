using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ApEn.Services
{
    /// <summary>
    /// Extrae el icono del sistema operativo asociado a un .exe.
    /// Usa System.Drawing.Common (paquete oficial Microsoft, firmado) para evitar
    /// falsos positivos de antivirus que se generan con P/Invoke manual a shell32.dll.
    /// </summary>
    public static class IconService
    {
        private static readonly Dictionary<string, ImageSource?> _cache =
            new(StringComparer.OrdinalIgnoreCase);

        public static ImageSource? GetIcon(string exePath)
        {
            if (_cache.TryGetValue(exePath, out var cached)) return cached;
            if (!File.Exists(exePath)) { _cache[exePath] = null; return null; }

            ImageSource? src = null;
            try
            {
                // Icon.ExtractAssociatedIcon es parte de System.Drawing.Common
                // (ensamblado oficial Microsoft, firmado — no dispara antivirus)
                using var icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                if (icon != null)
                {
                    src = Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromWidthAndHeight(32, 32));
                    src.Freeze(); // Hacer thread-safe
                }
            }
            catch { /* archivo sin icono o ruta inaccesible */ }

            _cache[exePath] = src;
            return src;
        }
    }
}
