using System;
using System.Security;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Runtime.CompilerServices;

namespace PicturesEditor.Model.Utils.SecurityCritical
{
    /// <summary>
    /// Представляет класс-оболочку для декстриптора GDI, созданного из Bitmap
    /// </summary>
    internal sealed class SafeHBitmapHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SafeHBitmapHandle"/>
        /// </summary>
        [SecurityCritical]
        public SafeHBitmapHandle(Bitmap bitmap) : base(true) => SetHandle(bitmap.GetHbitmap());

        /// <summary>
        /// Удаляет логическое перо, кисть, шрифт, растровое изображение, область или палитру, освобождая все системные ресурсы, связанные с объектом.
        /// <br/>После удаления объекта указанный дескриптор становится недействительным
        /// </summary>
        /// <param name="hObject">Дескриптор логического пера, кисти, шрифта, растрового изображения, области или палитры.</param>
        /// <returns>Возвращает <see langword="true"/>, если функция завершается успешно, иначе возвращает <see langword="false"/></returns>
        /// <remarks>
        ///   <para>Не удаляйте объект рисунка (перо или кисть), пока он выбран в контроллере домена.</para>
        ///   <para>
        ///   При удалении узорной ​​кисти растровое изображение, связанное с кистью, не удаляется.<br/>
        ///   Растровое изображение необходимо удалить самостоятельно.
        ///   </para>
        /// </remarks>
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        /// <inheritdoc />
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override bool ReleaseHandle() => DeleteObject(handle);
    }
}
