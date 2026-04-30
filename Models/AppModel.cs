namespace ApEn.Models
{
    public class AppModel
    {
        public string       Name       { get; set; } = string.Empty;
        public string       Path       { get; set; } = string.Empty;
        public bool         IsFavorite { get; set; }
        public List<string> Tags       { get; set; } = new();
    }
}
