using System.Windows;
using System.Windows.Input;

namespace TypeWriter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IDialogWindow
    {

        public Window1()
        {
            InitializeComponent();
        }

        public IDialogResult Result { get; set; }

        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.Hide();
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}