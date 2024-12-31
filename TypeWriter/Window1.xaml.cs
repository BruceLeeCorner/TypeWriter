using System.Windows;
using System.Windows.Input;

namespace TypeWriter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IDialogWindow
    {
        #region Public Constructors

        public Window1()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Properties

        public IDialogResult Result { get; set; }

        #endregion Properties

        #region Private Methods

        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.Hide();
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        #endregion Private Methods
    }
}