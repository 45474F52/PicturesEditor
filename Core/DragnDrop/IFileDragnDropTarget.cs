namespace PicturesEditor.Core.DragnDrop
{
    /// <summary>
    /// Определяет метод получения списка файлов
    /// </summary>
    internal interface IFileDragnDropTarget
    {
        /// <summary>
        /// Метод обработки списка файлов
        /// </summary>
        /// <param name="files">Массив абсолютных путей файлов</param>
        void OnFileDrop(string[] files);
    }
}
