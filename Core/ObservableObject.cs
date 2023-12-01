using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PicturesEditor.Core
{
    /// <summary>
    /// Предоставляет реализацию метода оповещения <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Вызывает событие <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// </summary>
        /// <param name="propertyName">Имя вызывающего свойства</param>
        /// <remarks>
        /// Аргумент <paramref name="propertyName"/> задаётся автоматически при помощи атрибута <see cref="CallerMemberNameAttribute"/>
        /// </remarks>
        protected internal virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
