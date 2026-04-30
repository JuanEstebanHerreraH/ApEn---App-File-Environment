namespace ApEn.Models
{
    public class ConfigModel
    {
        public List<AppModel> Apps           { get; set; } = new();
        public List<TagModel> Tags           { get; set; } = new();
        public string         LastFolderPath { get; set; } = string.Empty;
        public bool           IsDarkTheme    { get; set; } = true;
    }
}
