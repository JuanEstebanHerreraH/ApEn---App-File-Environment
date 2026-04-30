using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ApEn.ViewModels
{
    /// <summary>
    /// Base para todos los ViewModels. Implementa INotifyPropertyChanged
    /// con el helper SetProperty para reducir boilerplate.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Asigna un valor al campo de respaldo y dispara OnPropertyChanged
        /// solo si el valor realmente cambió. Devuelve true si hubo cambio.
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
