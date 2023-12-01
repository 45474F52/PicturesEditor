using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicturesEditor.Model
{
    /// <summary>
    /// Содержит методы применения фильтров к изображениям
    /// </summary>
    public static class PictureFiltering
    {
        /// <summary>
        /// Представляет изображение ImageSource в серых оттенках
        /// </summary>
        [Obsolete("Рекомендуется конвертировать ImageSource в Bitmap и применить аналогичный метод, но уже для Bitmap")]
        public static ImageSource AsGrayScale(this ImageSource imageSource)
        {
			try
			{
                FormatConvertedBitmap grayImage = new FormatConvertedBitmap();
                grayImage.BeginInit();
                grayImage.Source = imageSource as BitmapSource;
                grayImage.DestinationFormat = PixelFormats.Gray32Float;
                grayImage.EndInit();

                return grayImage;
            }
			catch (Exception ex)
			{
                MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                return default;
			}
        }

        /// <summary>
        /// Представляет изображение Bitmap в серых оттенках
        /// </summary>
        /// <remarks>Изменяет цвета пикселей при помощи цветовой матрицы (<see cref="ColorMatrix"/>)</remarks>
        public static Bitmap AsGrayScale(this Bitmap bitmap)
        {
            ColorMatrix grayScaleMatrix = new ColorMatrix(new float[][]
            {
                new float[] { 0.3f,     0.3f,     0.3f,     0f,    0f },
                new float[] { 0.59f,    0.59f,    0.59f,    0f,    0f },
                new float[] { 0.11f,    0.11f,    0.11f,    0f,    0f },
                new float[] { 0f,       0f,       0f,       1f,    0f },
                new float[] { 0f,       0f,       0f,       0f,    1f }
            });

            return bitmap.ApplyColorMatrix(grayScaleMatrix);
        }

        /// <summary>
        /// Представляет изображение Bitmap с инвертированными цветами пикселей
        /// </summary>
        /// <remarks>Инвертирует цвета пикселей при помощи цветовой матрицы (<see cref="ColorMatrix"/>)</remarks>
        public static Bitmap InvertColors(this Bitmap bitmap)
        {
            ColorMatrix invertMatrix = new ColorMatrix(new float[][]
            {
                new float[] { -1f,    0f,     0f,     0f,    0f },
                new float[] { 0f,     -1f,    0f,     0f,    0f },
                new float[] { 0f,     0f,     -1f,    0f,    0f },
                new float[] { 0f,     0f,     0f,     1f,    0f },
                new float[] { 1f,     1f,     1f,     0f,    1f },
            });

            return bitmap.ApplyColorMatrix(invertMatrix);
        }

        /// <summary>
        /// Применяет цветовую матрицу к изображению
        /// </summary>
        private static Bitmap ApplyColorMatrix(this Bitmap bitmap, ColorMatrix matrix)
        {
            using (ImageAttributes invertAttribute = new ImageAttributes())
            {
                invertAttribute.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(
                        bitmap,
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        0, 0,
                        bitmap.Width, bitmap.Height,
                        GraphicsUnit.Pixel,
                        invertAttribute);
                }
            }

            return bitmap;
        }
    }
}
