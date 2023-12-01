using PicturesEditor.Model.Utils.SecurityCritical;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PicturesEditor.Model.Utils
{
    /// <summary>
    /// Предоставляет методы расширения для изображений BitmapSource и Bitmap
    /// </summary>
    public static class ImageSourceHelper
    {
        /// <summary>
        /// Изменяет размер Bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="widthOffset">Смещение по ширине</param>
        /// <param name="maxWidth">Максимальная ширина</param>
        public static Bitmap Resize(this Bitmap bitmap, double widthOffset = 2d, int maxWidth = 200)
        {
            double newHeight = bitmap.Height / widthOffset * maxWidth / bitmap.Width;

            if (bitmap.Width > maxWidth || bitmap.Height > newHeight)
                bitmap = new Bitmap(bitmap, new System.Drawing.Size(maxWidth, (int)newHeight));

            return bitmap;
        }

        /// <summary>
        /// Представляет экземпляр класса BitmapSource как экземпляр Bitmap
        /// </summary>
        public static Bitmap AsBitmap(this BitmapSource source)
        {
            Bitmap bitmap = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bitmap.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        /// <summary>
        /// Представляет экземпляр класса Bitmap как экземпляр BitmapSource
        /// </summary>
        public static BitmapSource AsBitmapSource(this Bitmap bitmap)
        {
            using (SafeHBitmapHandle handle = new SafeHBitmapHandle(bitmap))
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    handle.DangerousGetHandle(),
                    IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
