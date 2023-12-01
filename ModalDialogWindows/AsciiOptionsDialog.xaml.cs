using System.Windows;

namespace PicturesEditor.ModalDialogWindows
{
    public partial class AsciiOptionsDialog : Window
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса AsciiOptionsDialog
        /// </summary>
        public AsciiOptionsDialog() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
