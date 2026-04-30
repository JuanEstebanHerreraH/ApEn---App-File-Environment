using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ApEn.Views
{
    public partial class TagDialog : Window
    {
        public string TagName  { get; private set; } = string.Empty;
        public string TagColor { get; private set; } = "#FF6C63FF";

        private static readonly string[] PresetColors =
        {
            "#FF6C63FF","#FF48C2A0","#FFFF5C6A","#FFFFB347",
            "#FF4FC3F7","#FFAB47BC","#FF26A69A","#FFEF5350"
        };

        private string _selectedColor = "#FF6C63FF";
        private Button? _activeBtn;
        private bool _updatingHex;

        public TagDialog()
        {
            InitializeComponent();
            BuildPresets();
            TxtHex.Text = _selectedColor;
            TxtName.Focus();
        }

        private void BuildPresets()
        {
            foreach (var hex in PresetColors)
            {
                var col = ParseColor(hex);
                var btn = new Button
                {
                    Width           = 28, Height = 28,
                    Margin          = new Thickness(0, 0, 6, 6),
                    Cursor          = Cursors.Hand,
                    BorderThickness = new Thickness(2.5),
                    BorderBrush     = Brushes.Transparent,
                    Background      = new SolidColorBrush(col),
                    Tag             = hex,
                    Template        = MakeCircleTemplate()
                };
                btn.Click += PresetBtn_Click;
                ColorPicker.Items.Add(btn);

                if (hex == _selectedColor)
                {
                    btn.BorderBrush = Brushes.White;
                    _activeBtn = btn;
                }
            }
        }

        private void PresetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            SelectColor(btn.Tag?.ToString() ?? "#FF6C63FF", btn);
        }

        private void SelectColor(string hex, Button? sourceBtn = null)
        {
            // Normalizar a 9 chars #FFRRGGBB
            if (!hex.StartsWith('#')) hex = "#" + hex;
            if (hex.Length == 7) hex = "#FF" + hex[1..];
            if (!IsValidHex(hex)) return;

            _selectedColor = hex;

            // Actualizar preview
            ColorPreview.Background = new SolidColorBrush(ParseColor(hex));

            // Deselect botón anterior
            if (_activeBtn != null) _activeBtn.BorderBrush = Brushes.Transparent;

            if (sourceBtn != null)
            {
                sourceBtn.BorderBrush = Brushes.White;
                _activeBtn = sourceBtn;
            }
            else
            {
                // Deseleccionar todos si vino del hex manual
                _activeBtn = null;
                foreach (Button b in ColorPicker.Items)
                    b.BorderBrush = Brushes.Transparent;
            }

            // Sync hex textbox sin loop
            if (!_updatingHex)
            {
                _updatingHex = true;
                TxtHex.Text = hex;
                _updatingHex = false;
            }
        }

        private void TxtHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updatingHex) return;
            var raw = TxtHex.Text.Trim();
            if (!raw.StartsWith('#')) raw = "#" + raw;
            if (raw.Length == 7) raw = "#FF" + raw[1..];
            if (IsValidHex(raw))
            {
                _updatingHex = true;
                SelectColor(raw);
                _updatingHex = false;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text)) { TxtName.Focus(); return; }
            TagName      = TxtName.Text.Trim();
            TagColor     = _selectedColor;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)  Create_Click(sender, e);
            if (e.Key == Key.Escape) Cancel_Click(sender, e);
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static bool IsValidHex(string hex)
        {
            if (hex.Length != 9 && hex.Length != 7) return false;
            try { ColorConverter.ConvertFromString(hex); return true; }
            catch { return false; }
        }

        private static Color ParseColor(string hex)
        {
            try { return (Color)(ColorConverter.ConvertFromString(hex) ?? Colors.Purple); }
            catch { return Colors.Purple; }
        }

        private static ControlTemplate MakeCircleTemplate()
        {
            var tpl     = new ControlTemplate(typeof(Button));
            var border  = new FrameworkElementFactory(typeof(Border));
            border.SetBinding(Border.BackgroundProperty,
                new System.Windows.Data.Binding("Background")
                { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetBinding(Border.BorderBrushProperty,
                new System.Windows.Data.Binding("BorderBrush")
                { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetBinding(Border.BorderThicknessProperty,
                new System.Windows.Data.Binding("BorderThickness")
                { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(14));
            tpl.VisualTree = border;
            return tpl;
        }
    }
}
