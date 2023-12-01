using PicturesEditor.Model.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;

namespace PicturesEditor.Model
{
    /// <summary>
    /// Предоставляет метод конвертации изображения в символы ASCII
    /// </summary>
    public static class PictureConvertering
    {
        /// <summary>
        /// Инкапсулирует опции для конвертации изображения
        /// </summary>
        public sealed class AsciiOptions
        {
            /// <summary>
            /// Изображения как экземпляр класса <see cref="Picture"/>
            /// </summary>
            public readonly Picture picture;
            /// <summary>
            /// Смещение по ширине
            /// </summary>
            public readonly double widthOffset;
            /// <summary>
            /// Максимальная ширина
            /// </summary>
            public readonly int maxWidth;
            /// <summary>
            /// Определяет, нужно ли использовать настройку "Высокое качество"
            /// </summary>
            public readonly bool hiqhQuality;
            /// <summary>
            /// Определяет то, какой цвет будет использоваться в терминале
            /// </summary>
            /// <remarks>
            /// Если равно <see langword="true"/>, то цвет фона терминала будет белым, а цвет шрифта — чёрным <br/>
            /// Иначе цвета будут инвертированы
            /// </remarks>
            public readonly bool fromBrightToDim;

            /// <summary>
            /// Инициализирует экземпляр класса <see cref="AsciiOptions"/>
            /// </summary>
            /// <param name="picture">Изображения как экземпляр класса <see cref="Picture"/></param>
            /// <param name="widthOffset">Смещение по ширине</param>
            /// <param name="maxWidth">Максимальная ширина</param>
            /// <param name="hiqhQuality">Определяет, нужно ли использовать настройку "Высокое качество"</param>
            /// <param name="fromBrightToDim">Определяет то, какой цвет будет использоваться в терминале</param>
            /// <exception cref="ArgumentException"></exception>
            public AsciiOptions(
                Picture picture,
                double widthOffset = 2d,
                int maxWidth = 200,
                bool hiqhQuality = false,
                bool fromBrightToDim = true)
            {
                if (widthOffset <= 0)
                    throw new ArgumentException("Недопустимое значение для смещения по ширине!", nameof(widthOffset));
                if (maxWidth <= 0)
                    throw new ArgumentException("Недопустимое значение для максимальной ширины!", nameof(maxWidth));

                if (widthOffset < 2d)
                    widthOffset = 2d;

                this.widthOffset = widthOffset;
                this.maxWidth = maxWidth;
                this.picture = picture;
                this.hiqhQuality = hiqhQuality;
                this.fromBrightToDim = fromBrightToDim;
            }

            /// <summary>
            /// Инициализирует экземпляр класса <see cref="AsciiOptions"/>
            /// </summary>
            /// <param name="picture">Изображения как экземпляр класса <see cref="Picture"/></param>
            /// <param name="tuple">Коллаж параметров из альтернативного конструктора класса</param>
            public AsciiOptions(Picture picture, (double widthOffset, int maxWidth, bool highQuality, bool fromBrightToDim) tuple) :
                this(picture, tuple.widthOffset, tuple.maxWidth, tuple.highQuality, tuple.fromBrightToDim) { }
        }

        /// <summary>
        /// Конвертирует изображения в ASCII-символы
        /// </summary>
        /// <param name="options">Опции для конвертации изображения</param>
        public static void ConvertToAscii(AsciiOptions options)
        {
            Bitmap bitmap = options.picture.Bitmap.Resize(options.widthOffset, options.maxWidth);
            if ((options.picture.Filters & PictureFilters.GrayScaled) != PictureFilters.GrayScaled)
                bitmap = bitmap.AsGrayScale();

            char[][] rows = BitmapToAsciiConverter.Convert(bitmap, options.hiqhQuality, options.fromBrightToDim);

            StringBuilder sb = new StringBuilder();
            foreach (char[] row in rows)
                sb.AppendLine(new string(row));

            int back = options.fromBrightToDim ? 7 : 0;
            int front = options.fromBrightToDim ? 0 : 7;

            string name = Path.GetFileNameWithoutExtension(options.picture.File.FullName);
            string subname = options.hiqhQuality ? " Ultra" : "";
            string pathToSaveFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                string.Format("{0}{1}.txt", name, subname));
            try
            {
                File.WriteAllText(pathToSaveFile, sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.TargetSite.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            string args = CMDArgumentsBuilder.CreateBuilder(specificCommand: "/K ")
                .Title(pathToSaveFile)
                .Color(back, front)
                .ClearText()
                .ReadFile(pathToSaveFile)
                .OpenFile(pathToSaveFile)
                .Build();

            System.Diagnostics.Process cmd = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "cmd",
                    Arguments = args,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized,
                    UseShellExecute = true
                }
            };

            cmd.Start();
        }
    }
}
