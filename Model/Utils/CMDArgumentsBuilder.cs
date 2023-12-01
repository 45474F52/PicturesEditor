using System;
using System.Text;

namespace PicturesEditor.Model.Utils
{
    /// <summary>
    /// Создаёт перечень аргументов для командной строки
    /// </summary>
    internal sealed class CMDArgumentsBuilder
    {
        private readonly StringBuilder _argumentsBuilder = new StringBuilder();

        private CMDArgumentsBuilder() { }

        /// <summary>
        /// Создаёт экземпляр класса <see cref="CMDArgumentsBuilder"/>
        /// </summary>
        /// <param name="specificCommand">Необязательная команда, предшествующая остальным</param>
        /// <returns></returns>
        public static CMDArgumentsBuilder CreateBuilder(string specificCommand = "")
        {
            CMDArgumentsBuilder builder = new CMDArgumentsBuilder();
            builder.AppendCommand(specificCommand);
            return builder;
        }

        /// <summary>
        /// Добавляет команду к перечню команд
        /// </summary>
        /// <remarks>
        /// Если были команды ранее, то новая добавляется в формате:
        /// <code>
        /// stringBuilder.AppendFormat($" &amp; {command}");
        /// </code>
        /// </remarks>
        public void AppendCommand(string command)
        {
            if (_argumentsBuilder.Length > 3)
                _argumentsBuilder.AppendFormat($" & {command}");
            else
                _argumentsBuilder.Append(command);
        }

        /// <summary>
        /// Возвращает перечень команд в виде строки
        /// </summary>
        public string Build() => _argumentsBuilder.ToString();
    }

    /// <summary>
    /// Расширяет методы добавления аргументов класса <see cref="CMDArgumentsBuilder"/>
    /// </summary>
    internal static class CMDArgumentsBuilderExtensions
    {
        /// <summary>
        /// Добавляет произвольную команду
        /// </summary>
        public static CMDArgumentsBuilder Custom(this CMDArgumentsBuilder builder, string command)
        {
            builder.AppendCommand(command);
            return builder;
        }

        /// <summary>
        /// Задаёт заголовок терминала
        /// </summary>
        public static CMDArgumentsBuilder Title(this CMDArgumentsBuilder builder, string title)
        {
            builder.AppendCommand($"title {title}");
            return builder;
        }

        /// <summary>
        /// Задаёт цвет терминала (цвет фона и цвет шрифта)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="back">Значение цвета фона</param>
        /// <param name="front">Значение цвета шрифта</param>
        /// <remarks>
        /// Цвета задаются как числа в диапазоне от 0 до 15 <br/>
        /// Цвет шрифта и фона не может быть одинаковым!
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static CMDArgumentsBuilder Color(this CMDArgumentsBuilder builder, int back, int front)
        {
            if (back < 0 || back > 15)
                throw new ArgumentOutOfRangeException(nameof(back));
            if (front < 0 || front > 15)
                throw new ArgumentOutOfRangeException(nameof(front));
            if (back == front)
                throw new ArgumentException("Цвета переднего и заднего плана не могут быть одинаковыми");

            string backHex, frontHex;
            backHex = Convert.ToString(back, 16);
            frontHex = Convert.ToString(front, 16);

            builder.AppendCommand($"color {backHex}{frontHex}");
            return builder;
        }

        /// <summary>
        /// Очищает терминал
        /// </summary>
        public static CMDArgumentsBuilder ClearText(this CMDArgumentsBuilder builder)
        {
            builder.AppendCommand("cls");
            return builder;
        }

        /// <summary>
        /// Копирует содержимое файла в терминал
        /// </summary>
        public static CMDArgumentsBuilder ReadFile(this CMDArgumentsBuilder builder, string file)
        {
            builder.AppendCommand($"type \"{file}\"");
            return builder;
        }

        /// <summary>
        /// Открывает файл (запускает новый процесс)
        /// </summary>
        public static CMDArgumentsBuilder OpenFile(this CMDArgumentsBuilder builder, string file)
        {
            builder.AppendCommand($"\"{file}\"");
            return builder;
        }
    }
}
