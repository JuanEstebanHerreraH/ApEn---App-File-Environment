using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using ApEn.Commands;
using ApEn.Models;
using ApEn.Services;

namespace ApEn.ViewModels
{
    public class AppItemViewModel : BaseViewModel
    {
        public AppModel Model { get; }
        private readonly Action _onChanged;
        private readonly Action? _onFavChanged; // Notifica al VM para refrescar vistas filtradas

        private bool   _isEditing;
        private string _editName = string.Empty;
        private ImageSource? _icon;
        private bool   _iconLoaded;

        public string Name
        {
            get => Model.Name;
            set { Model.Name = value; OnPropertyChanged(); _onChanged(); }
        }

        public string Path => Model.Path;

        public bool IsFavorite
        {
            get => Model.IsFavorite;
            set { Model.IsFavorite = value; OnPropertyChanged(); _onChanged(); _onFavChanged?.Invoke(); }
        }

        /// <summary>Icono real del .exe, extraído en primera demanda.</summary>
        public ImageSource? Icon
        {
            get
            {
                if (!_iconLoaded) { _icon = IconService.GetIcon(Model.Path); _iconLoaded = true; }
                return _icon;
            }
        }

        public bool HasIcon => Icon != null;

        public ObservableCollection<string> Tags { get; }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public string EditName
        {
            get => _editName;
            set => SetProperty(ref _editName, value);
        }

        public ICommand StartEditCommand  { get; }
        public ICommand CommitEditCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand ToggleFavCommand  { get; }
        public ICommand RemoveTagCommand  { get; }

        public AppItemViewModel(AppModel model, Action onChanged, Action? onFavChanged = null)
        {
            Model          = model;
            _onChanged     = onChanged;
            _onFavChanged  = onFavChanged;

            Tags = new ObservableCollection<string>(model.Tags);
            Tags.CollectionChanged += (_, _) =>
            {
                model.Tags = Tags.ToList();
                _onChanged();
            };

            StartEditCommand  = new RelayCommand(_ => StartEdit());
            CommitEditCommand = new RelayCommand(_ => CommitEdit(), _ => !string.IsNullOrWhiteSpace(EditName));
            CancelEditCommand = new RelayCommand(_ => IsEditing = false);
            ToggleFavCommand  = new RelayCommand(_ => IsFavorite = !IsFavorite);
            RemoveTagCommand  = new RelayCommand(t => { if (t is string s) Tags.Remove(s); });
        }

        private void StartEdit() { EditName = Model.Name; IsEditing = true; }
        private void CommitEdit() { Name = EditName.Trim(); IsEditing = false; }

        public void AddTag(string tagName)
        {
            if (!Tags.Contains(tagName)) Tags.Add(tagName);
        }
    }
}
