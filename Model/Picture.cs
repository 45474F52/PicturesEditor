using PicturesEditor.Core;
using PicturesEditor.Model.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicturesEditor.Model
{
    /// <summary>
    /// Представляет собой объект для взаимодействия с изображением как с экземпляром класса
    /// <see cref="System.Windows.Media.ImageSource"/> и/или <see cref="System.Drawing.Bitmap"/>
    /// </summary>
    public sealed class Picture : ObservableObject, IDisposable
    {
        private FileInfo _file;
        private ImageSource _imageSource;
        private Bitmap _bitmap;

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="Picture"/>
        /// </summary>
        /// <param name="file">Путь до файла изображения</param>
        /// <exception cref="FileNotFoundException"></exception>
        public Picture(string file)
        {
            if (System.IO.File.Exists(file))
            {
                _file = new FileInfo(file);
                _imageSource = new BitmapImage(new Uri(file));
                _bitmap = new Bitmap(file);
                _filters = PictureFilters.Default;
            }
            else
                throw new FileNotFoundException("Файл не существует или имеет ограничения на доступ к нему", file);
        }

        private PictureFilters _filters;
        /// <summary>
        /// Фильтры изображения
        /// </summary>
        public PictureFilters Filters
        {
            get => _filters;
            set
            {
                _filters = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Предоставляет экземпляр Picture как файл
        /// </summary>
        public FileInfo File => _file;
        /// <summary>
        /// Предоставляет экземпляр Picture как ImageSource
        /// </summary>
        public ImageSource ImageSource => _imageSource;
        /// <summary>
        /// Предоставляет экземпляр Picture как Bitmap
        /// </summary>
        public Bitmap Bitmap => _bitmap;

        /// <summary>
        /// Изменяет внутреннее значение свойства bitmap и imageSource
        /// </summary>
        /// <param name="imageSource"></param>
        /// <remarks>
        /// Преобразует imageSource в bitmap (<see cref="ImageSourceHelper.AsBitmap(BitmapSource)"/>)
        /// </remarks>
        public void Refresh(ImageSource imageSource)
        {
            _imageSource = imageSource;
            _bitmap = (_imageSource as BitmapSource).AsBitmap();
        }

        /// <summary>
        /// Изменяет внутреннее значение свойства bitmap и imageSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <remarks>
        /// Преобразует bitmap в imageSource (<see cref="ImageSourceHelper.AsBitmapSource(Bitmap)"/>)
        /// </remarks>
        public void Refresh(Bitmap bitmap)
        {
            _bitmap = bitmap;
            _imageSource = _bitmap.AsBitmapSource();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _bitmap.Dispose();
            _imageSource = null;
            _file = null;
            _filters = PictureFilters.None;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Возвращает свойство <see cref="ImageSource"/>
        /// </summary>
        public static implicit operator ImageSource(Picture picture) => picture.ImageSource;
        /// <summary>
        /// Возвращает свойство <see cref="Bitmap"/>
        /// </summary>
        public static implicit operator Bitmap(Picture picture) => picture.Bitmap;
    }

    /// <summary>
    /// Фильтры изображения
    /// </summary>
    [Flags]
    public enum PictureFilters
    {
        /// <summary>
        /// Отсутсвие фильтров
        /// </summary>
        None =              0b_0000,
        /// <summary>
        /// Фильтры "по умолчанию"
        /// </summary>
        Default =           0b_0001,
        /// <summary>
        /// Оттенки серого
        /// </summary>
        GrayScaled =        0b_0010,
        /// <summary>
        /// Инвертированные цвета
        /// </summary>
        InvertedColors =    0b_0100,
    }
}
