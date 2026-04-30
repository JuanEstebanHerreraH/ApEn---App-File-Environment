using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ApEn.Commands;
using ApEn.Models;
using ApEn.Services;
using Microsoft.Win32;

namespace ApEn.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ConfigService        _cfg  = new();
        private readonly FileOrganizerService _forg = new();

        // ── backing fields ────────────────────────────────────────────────────
        private AppItemViewModel? _selectedApp;
        private string  _selectedFolderPath = string.Empty;
        private string  _statusMessage      = "Listo.";
        private bool    _isBusy;
        private int     _activeTab;          // 0=Launcher, 1=Organizer
        private bool    _isDark             = true;
        private bool    _showTagPanel;
        private bool    _showFavorites = true;
        private bool    _showOrganizeResult;
        private string  _organizeResultMsg  = string.Empty;
        private bool    _organizeResultIsOk = true;

        // ── colecciones ───────────────────────────────────────────────────────
        public ObservableCollection<AppItemViewModel> Apps       { get; } = new();
        public ObservableCollection<TagModel>         AllTags    { get; } = new();
        public ObservableCollection<string>           ActiveTagFilters { get; } = new();

        // Vistas filtradas
        private readonly CollectionViewSource _favSrc = new();
        private readonly CollectionViewSource _appSrc = new();
        public ICollectionView FavoritesView => _favSrc.View;
        public ICollectionView AppsView      => _appSrc.View;

        public static List<string> TagColors { get; } = new()
        {
            "#FF6C63FF","#FF48C2A0","#FFFF5C6A","#FFFFB347",
            "#FF4FC3F7","#FFAB47BC","#FFEF5350","#FF26A69A"
        };

        // ── propiedades ───────────────────────────────────────────────────────
        public AppItemViewModel? SelectedApp
        {
            get => _selectedApp;
            set { if (SetProperty(ref _selectedApp, value)) OnPropertyChanged(nameof(HasSelectedApp)); }
        }
        public bool HasSelectedApp => _selectedApp != null;

        public string SelectedFolderPath
        {
            get => _selectedFolderPath;
            set => SetProperty(ref _selectedFolderPath, value);
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        public int ActiveTab
        {
            get => _activeTab;
            set => SetProperty(ref _activeTab, value);
        }
        public bool IsDark
        {
            get => _isDark;
            set => SetProperty(ref _isDark, value);
        }
        public bool ShowTagPanel
        {
            get => _showTagPanel;
            set => SetProperty(ref _showTagPanel, value);
        }

        public bool ShowFavorites
        {
            get => _showFavorites;
            set => SetProperty(ref _showFavorites, value);
        }

        public bool ShowOrganizeResult
        {
            get => _showOrganizeResult;
            set => SetProperty(ref _showOrganizeResult, value);
        }
        public string OrganizeResultMsg
        {
            get => _organizeResultMsg;
            set => SetProperty(ref _organizeResultMsg, value);
        }
        public bool OrganizeResultIsOk
        {
            get => _organizeResultIsOk;
            set => SetProperty(ref _organizeResultIsOk, value);
        }

        // ── comandos ─────────────────────────────────────────────────────────
        public ICommand AddAppCommand           { get; }
        public ICommand LaunchAppCommand        { get; }
        public ICommand RemoveAppCommand        { get; }
        public ICommand BrowseFolderCommand     { get; }
        public ICommand OrganizeFilesCommand    { get; }
        public ICommand SwitchTabCommand        { get; }
        public ICommand ToggleTagFilterCommand  { get; }
        public ICommand ClearTagFiltersCommand  { get; }
        public ICommand AddTagToSelectedCommand { get; }
        public ICommand DeleteTagCommand        { get; }
        public ICommand ToggleThemeCommand      { get; }
        public ICommand DismissResultCommand    { get; }
        public ICommand ToggleTagPanelCommand   { get; }
        public ICommand ToggleFavoritesCommand { get; }

        // Callbacks para abrir diálogos desde la vista
        public Action? RequestOpenTagDialog { get; set; }

        // ── constructor ───────────────────────────────────────────────────────
        public MainViewModel()
        {
            _favSrc.Source = Apps;
            _appSrc.Source = Apps;
            _favSrc.Filter += (_, e) => FilterApp(e, favoritesOnly: true);
            _appSrc.Filter += (_, e) => FilterApp(e, favoritesOnly: false);

            AddAppCommand           = new RelayCommand(_ => AddApp());
            LaunchAppCommand        = new RelayCommand(p => LaunchApp(p as AppItemViewModel), p => p is AppItemViewModel);
            RemoveAppCommand        = new RelayCommand(p => RemoveApp(p as AppItemViewModel), p => p is AppItemViewModel);
            BrowseFolderCommand     = new RelayCommand(_ => BrowseFolder());
            OrganizeFilesCommand    = new RelayCommand(_ => OrganizeFiles(),
                                          _ => !string.IsNullOrWhiteSpace(SelectedFolderPath) && !IsBusy);
            SwitchTabCommand        = new RelayCommand(p => ActiveTab = Convert.ToInt32(p));
            ToggleTagFilterCommand  = new RelayCommand(p => ToggleTagFilter(p as string));
            ClearTagFiltersCommand  = new RelayCommand(_ => { ActiveTagFilters.Clear(); Refresh(); });
            AddTagToSelectedCommand = new RelayCommand(p => AddTagToSelected(p as string),
                                          p => SelectedApp != null && p is string s && !SelectedApp.Tags.Contains(s));
            DeleteTagCommand        = new RelayCommand(p => DeleteTag(p as TagModel), p => p is TagModel);
            ToggleThemeCommand      = new RelayCommand(_ => ToggleTheme());
            DismissResultCommand    = new RelayCommand(_ => ShowOrganizeResult = false);
            ToggleTagPanelCommand   = new RelayCommand(_ => ShowTagPanel = !ShowTagPanel);
            ToggleFavoritesCommand  = new RelayCommand(_ => ShowFavorites = !ShowFavorites);

            LoadConfig();
        }

        // ── tags ──────────────────────────────────────────────────────────────
        public void CreateTag(string name, string color)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (AllTags.Any(t => t.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))) return;
            AllTags.Add(new TagModel { Name = name.Trim(), Color = color });
            SaveConfig();
        }

        private void DeleteTag(TagModel? tag)
        {
            if (tag is null) return;
            AllTags.Remove(tag);
            ActiveTagFilters.Remove(tag.Name);
            foreach (var app in Apps) app.Tags.Remove(tag.Name);
            Refresh();
            SaveConfig();
        }

        private void ToggleTagFilter(string? tag)
        {
            if (tag is null) return;
            if (ActiveTagFilters.Contains(tag)) ActiveTagFilters.Remove(tag);
            else ActiveTagFilters.Add(tag);
            Refresh();
            OnPropertyChanged(nameof(ActiveTagFilters));
        }

        private void AddTagToSelected(string? tagName)
        {
            if (tagName is null || SelectedApp is null) return;
            SelectedApp.AddTag(tagName);
            Refresh();
        }

        // ── launcher ──────────────────────────────────────────────────────────
        private void AddApp()
        {
            var dialog = new OpenFileDialog
            {
                Title  = "Seleccionar aplicacion",
                Filter = "Ejecutables (*.exe)|*.exe|Accesos directos (*.lnk)|*.lnk|Todos (*.*)|*.*"
            };
            if (dialog.ShowDialog() != true) return;
            if (Apps.Any(a => a.Path.Equals(dialog.FileName, StringComparison.OrdinalIgnoreCase)))
            { StatusMessage = "Esa aplicacion ya esta en la lista."; return; }

            var model = new AppModel { Name = Path.GetFileNameWithoutExtension(dialog.FileName), Path = dialog.FileName };
            var vm    = new AppItemViewModel(model, SaveConfig, Refresh);
            Apps.Add(vm);
            SelectedApp = vm;
            Refresh();
            SaveConfig();
            StatusMessage = $"'{model.Name}' agregada.";
        }

        private void LaunchApp(AppItemViewModel? app)
        {
            if (app is null) return;
            try { Process.Start(new ProcessStartInfo(app.Path) { UseShellExecute = true }); StatusMessage = $"Lanzando '{app.Name}'..."; }
            catch (Exception ex) { MessageBox.Show($"No se pudo iniciar:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void RemoveApp(AppItemViewModel? app)
        {
            if (app is null) return;
            var name = app.Name;
            Apps.Remove(app);
            if (SelectedApp == app) SelectedApp = null;
            Refresh();
            SaveConfig();
            StatusMessage = $"'{name}' eliminada.";
        }

        // ── organizer ─────────────────────────────────────────────────────────
        private void BrowseFolder()
        {
            var dialog = new OpenFolderDialog { Title = "Selecciona la carpeta a organizar" };
            if (!string.IsNullOrWhiteSpace(SelectedFolderPath) && Directory.Exists(SelectedFolderPath))
                dialog.InitialDirectory = SelectedFolderPath;
            if (dialog.ShowDialog() == true)
            {
                SelectedFolderPath = dialog.FolderName;
                ShowOrganizeResult = false;
                SaveConfig();
                StatusMessage = $"Carpeta: {SelectedFolderPath}";
            }
        }

        private async void OrganizeFiles()
        {
            if (!Directory.Exists(SelectedFolderPath))
            { MessageBox.Show("La carpeta no existe.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            IsBusy = true; ShowOrganizeResult = false; StatusMessage = "Organizando archivos...";
            FileOrganizerService.OrganizeResult result = default!;
            await Task.Run(() => result = _forg.Organize(SelectedFolderPath));
            IsBusy = false;
            OrganizeResultIsOk = result.Errors == 0;
            OrganizeResultMsg  = result.Errors == 0
                ? $"Completado — {result.Moved} archivos movidos"
                : $"Parcial — {result.Moved} movidos · {result.Errors} errores";
            ShowOrganizeResult = true;
            StatusMessage = OrganizeResultMsg;
        }

        private void ToggleTheme() { ThemeService.Toggle(); IsDark = ThemeService.IsDark; SaveConfig(); }

        private void FilterApp(FilterEventArgs e, bool favoritesOnly)
        {
            if (e.Item is not AppItemViewModel app) { e.Accepted = false; return; }

            if (favoritesOnly)
            {
                // Sección Favoritos: solo apps marcadas como favorito (sin filtro de tags)
                e.Accepted = app.IsFavorite;
                return;
            }

            // Sección Todas: filtrar por tags si hay filtros activos
            bool passTag = ActiveTagFilters.Count == 0 || ActiveTagFilters.All(t => app.Tags.Contains(t));
            e.Accepted   = passTag;
        }

        private void Refresh()
        {
            _favSrc.View.Refresh();
            _appSrc.View.Refresh();
            OnPropertyChanged(nameof(FavoritesView));
            OnPropertyChanged(nameof(AppsView));
        }

        private void LoadConfig()
        {
            var config = _cfg.Load();
            ThemeService.Apply(config.IsDarkTheme);
            IsDark = config.IsDarkTheme;
            foreach (var tag in config.Tags) AllTags.Add(tag);
            foreach (var app in config.Apps) Apps.Add(new AppItemViewModel(app, SaveConfig, Refresh));
            SelectedFolderPath = config.LastFolderPath;
        }

        public void SaveConfig()
        {
            _cfg.Save(new ConfigModel
            {
                Apps           = Apps.Select(a => a.Model).ToList(),
                Tags           = AllTags.ToList(),
                LastFolderPath = SelectedFolderPath,
                IsDarkTheme    = IsDark
            });
        }
    }
}
