using System.IO;
using System.Text.Json;
using ApEn.Models;

namespace ApEn.Services
{
    /// <summary>
    /// Gestiona la persistencia en Data/config.json.
    /// El archivo se guarda junto al ejecutable para portabilidad.
    /// </summary>
    public class ConfigService
    {
        private static readonly string DataDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        private static readonly string ConfigPath =
            Path.Combine(DataDir, "config.json");

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            WriteIndented        = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>Carga la configuración. Devuelve un modelo vacío si no existe el archivo.</summary>
        public ConfigModel Load()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return new ConfigModel();
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<ConfigModel>(json, JsonOpts) ?? new ConfigModel();
            }
            catch
            {
                // Si el JSON está corrupto arrancamos con defaults
                return new ConfigModel();
            }
        }

        /// <summary>Serializa y guarda la configuración completa.</summary>
        public void Save(ConfigModel config)
        {
            Directory.CreateDirectory(DataDir);
            var json = JsonSerializer.Serialize(config, JsonOpts);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
