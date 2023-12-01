using System.Linq;

namespace PicturesEditor.Model.Utils
{
    internal static class BitmapToAsciiConverter
    {
        public static char[][] Convert(System.Drawing.Bitmap bitmap, bool highQuality, bool fromBrightToDim)
        {
            char[][] result = new char[bitmap.Height][];

            string ascii = GetAsciiTable(highQuality, fromBrightToDim);

            for (int i = 0; i < bitmap.Height; i++)
            {
                result[i] = new char[bitmap.Width];

                for (int j = 0; j < bitmap.Width; j++)
                {
                    int mapIndex = (int)Map(
                        valueToMap: bitmap.GetPixel(j, i).R, start1: 0f, stop1: 255f, start2: 0f,
                        stop2: ascii.Length - 1);

                    result[i][j] = ascii[mapIndex];
                }
            }

            return result;
        }

        /// <summary>
        /// Преобразует значение из одного диапазона в другой
        /// </summary>
        /// <param name="valueToMap">Исходное значение, которое нужно преобразовать к другому диапазону</param>
        /// <param name="start1">Начало изначального диапазона</param>
        /// <param name="stop1">Конец изначального диапазона</param>
        /// <param name="start2">Начало нужного диапазона</param>
        /// <param name="stop2">Конец нужного диапазона</param>
        /// <returns>Возвращает исходное значение в новом диапазоне</returns>
        private static float Map(float valueToMap, float start1, float stop1, float start2, float stop2) =>
            (valueToMap - start1) / (stop1 - start1) * (stop2 - start2) + start2;

        private static string GetAsciiTable(bool highQuality, bool fromBrightToDim)
        {
            string table = highQuality ? "@BNR#8Q&$GXZA52S%3FCJ17L?!*:,." : "@#%?*+:,.";

            if (!fromBrightToDim)
                table = new string(table.Reverse().ToArray());

            return table;
        }
    }
}
