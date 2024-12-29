using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TypeWriter.UserInterface
{
    /// <summary>
    /// Interaction logic for LearnWordView.xaml
    /// </summary>
    public partial class LearnWordView : UserControl
    {
        public LearnWordView()
        {
            InitializeComponent();
        }

        private void TextBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).Focus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox.Focus();
        }
    }
}