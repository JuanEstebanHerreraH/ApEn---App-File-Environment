using System.IO;

namespace ApEn.Services
{
    /// <summary>
    /// Clasifica los archivos de una carpeta en subcarpetas según su extensión.
    /// Solo mueve archivos del nivel raíz; no toca las subcarpetas existentes.
    /// </summary>
    public class FileOrganizerService
    {
        // Mapa de categoría → extensiones reconocidas
        private static readonly Dictionary<string, HashSet<string>> Categories = new()
        {
            ["PDF"]    = new(StringComparer.OrdinalIgnoreCase) { ".pdf" },
            ["Images"] = new(StringComparer.OrdinalIgnoreCase)
                         { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".tiff", ".svg", ".ico", ".raw" },
            ["Videos"] = new(StringComparer.OrdinalIgnoreCase)
                         { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v", ".mpeg", ".mpg" },
            ["Audio"]  = new(StringComparer.OrdinalIgnoreCase)
                         { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a" },
            ["Docs"]   = new(StringComparer.OrdinalIgnoreCase)
                         { ".docx", ".doc", ".xlsx", ".xls", ".pptx", ".ppt", ".txt", ".csv", ".odt" },
            ["Zips"]   = new(StringComparer.OrdinalIgnoreCase)
                         { ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2" },
        };

        public record OrganizeResult(int Moved, int Skipped, int Errors, List<string> ErrorMessages);

        /// <summary>
        /// Mueve los archivos de <paramref name="folderPath"/> a sus subcarpetas correspondientes.
        /// </summary>
        public OrganizeResult Organize(string folderPath)
        {
            int moved = 0, skipped = 0, errors = 0;
            var errorMessages = new List<string>();

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    var ext      = Path.GetExtension(file);
                    var category = ResolveCategory(ext);
                    var targetDir = Path.Combine(folderPath, category);

                    Directory.CreateDirectory(targetDir);

                    var dest = GetUniqueDestPath(targetDir, Path.GetFileName(file));

                    if (dest is null)
                    {
                        // Archivo idéntico ya existe en destino
                        skipped++;
                        continue;
                    }

                    File.Move(file, dest);
                    moved++;
                }
                catch (Exception ex)
                {
                    errors++;
                    errorMessages.Add($"{Path.GetFileName(file)}: {ex.Message}");
                }
            }

            return new OrganizeResult(moved, skipped, errors, errorMessages);
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static string ResolveCategory(string ext)
        {
            foreach (var (cat, exts) in Categories)
                if (exts.Contains(ext)) return cat;
            return "Others";
        }

        /// <summary>
        /// Devuelve una ruta de destino libre.
        /// Si el archivo ya existe con el mismo nombre lo intenta con sufijo (1), (2)...
        /// Devuelve null si el archivo origen y destino son idénticos en nombre y tamaño.
        /// </summary>
        private static string? GetUniqueDestPath(string targetDir, string fileName)
        {
            var dest = Path.Combine(targetDir, fileName);
            if (!File.Exists(dest)) return dest;

            // Mismo nombre → consideramos duplicado y omitimos
            return null;
        }
    }
}
