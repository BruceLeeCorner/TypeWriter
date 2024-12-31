using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TypeWriter.UserInterface
{
    /// <summary>
    /// Interaction logic for LearnWordView.xaml
    /// </summary>
    public partial class LearnWordView : UserControl
    {
        #region Public Constructors

        public LearnWordView()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        private void TextBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox.Focus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox.Focus();
        }

        #endregion Private Methods
    }
}