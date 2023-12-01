using Microsoft.Win32;
using PicturesEditor.Core;
using PicturesEditor.ModalDialogWindows;
using PicturesEditor.Model;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicturesEditor
{
    internal sealed class MainViewModel : ObservableObject, Core.DragnDrop.IFileDragnDropTarget
    {
        private const string FILTER = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg|JPEG (*.jpeg)|*.jpeg|BMP (*.bmp)|*.bmp|All Files (*.*)|*.*";

        private Picture _picture;

        public ImageSource Image
        {
            get
            {
                if (_picture != null)
                    return _picture;
                return default;
            }
            private set
            {
                _picture.Refresh(value);
                OnPropertyChanged();
            }
        }

        private object _modalDialog;
        public object ModalDialog
        {
            get => _modalDialog;
            set
            {
                _modalDialog = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SettingCommand { get; private set; }

        public RelayCommand LoadImageCommand { get; private set; }
        public RelayCommand ResetFiltersCommand { get; private set; }
        public RelayCommand DeleteImageCommand { get; private set; }
        public RelayCommand SaveImageCommand { get; private set; }

        public RelayCommand ApplyGrayScaleCommand { get; private set; }
        public RelayCommand InvertColorsCommand { get; private set; }

        public RelayCommand ConvertToAsciiCommand { get; private set; }

        public MainViewModel()
        {
            LoadImageCommand = new RelayCommand(_ =>
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = FILTER,
                    DefaultExt = ".png",
                    Multiselect = false,
                    Title = "Выберите изображение"
                };

                if (dialog.ShowDialog() == true)
                    OnFileDrop(dialog.FileNames);
            });

            ResetFiltersCommand = new RelayCommand(_ => OnFileDrop(new[] { _picture.File.FullName }), __ => _picture != default);

            DeleteImageCommand = new RelayCommand(_ =>
            {
                _picture.Dispose();
                _picture = default;
                OnPropertyChanged(nameof(Image));
            }, __ => _picture != default);

            SaveImageCommand = new RelayCommand(_ =>
            {
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    Filter = FILTER,
                    OverwritePrompt = true,
                    CheckPathExists = true,
                    FileName = Path.GetFileNameWithoutExtension(_picture.File.FullName),
                    Title = "Сохраните изображение"
                };

                if (dialog.ShowDialog() == true)
                {
                    string extension = Path.GetExtension(dialog.FileName);

                    using (var stream = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        BitmapEncoder encoder = default;

                        switch (extension)
                        {
                            case ".png":
                                encoder = new PngBitmapEncoder();
                                break;
                            case ".jpg":
                                encoder = new JpegBitmapEncoder();
                                break;
                            case ".bmp":
                                encoder = new BmpBitmapEncoder();
                                break;
                            default:
                                MessageBox.Show(
                                        "Расширение изображения может быть только .png, .jpg//.jpeg или .bmp",
                                        "SaveImageCommand",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                                break;
                        }

                        encoder.Frames.Add(BitmapFrame.Create(Image as BitmapSource));
                        encoder.Save(stream);
                    }

                    MessageBox.Show(
                        string.Format("Изображение сохранено по пути:{0}\"{1}\"", Environment.NewLine, dialog.FileName),
                        "Сохранение изображения прошло успешно!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }, __ => _picture != default);

            ApplyGrayScaleCommand = new RelayCommand(_ =>
            {
                _picture.Refresh(_picture.Bitmap.AsGrayScale());
                _picture.Filters |= PictureFilters.GrayScaled;
                OnPropertyChanged(nameof(Image));
            }, __ => _picture != default && (_picture.Filters & PictureFilters.GrayScaled) != PictureFilters.GrayScaled);

            InvertColorsCommand = new RelayCommand(_ =>
            {
                _picture.Refresh(_picture.Bitmap.InvertColors());
                _picture.Filters |= PictureFilters.InvertedColors;
                OnPropertyChanged(nameof(Image));
            }, __ => _picture != default);

            ConvertToAsciiCommand = new RelayCommand(_ =>
            {
                AsciiOptionsDialogVM dialog = new AsciiOptionsDialogVM();

                dialog.OnApply += result => PictureConvertering.ConvertToAscii(new PictureConvertering.AsciiOptions(_picture, result));

                ModalDialog = dialog;
            }, __ => _picture != default);
        }

        public void OnFileDrop(string[] files)
        {
            if (files.Any())
            {
                _picture = new Picture(files.Last());
                OnPropertyChanged(nameof(Image));
            }
        }
    }
}
