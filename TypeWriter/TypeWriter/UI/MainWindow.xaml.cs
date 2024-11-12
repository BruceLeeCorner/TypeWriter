using CommunityToolkit.Mvvm.Messaging;
using DryIoc.Messages;
using Prism.Ioc;
using System.Windows;
using System.Windows.Input;

namespace TypeWriter.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.Instance.Container.Resolve<IMessenger>().Register<string, string>(this, "show_typebox", (o, m) =>
            {
                this.Show();

                TextBlock.Focus();
            });

            App.Instance.Container.Resolve<IMessenger>().Register<string, string>(this, "hide", (o, m) =>
            {
                this.Hide();
            });
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock.Focus();
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock.Focus();
            this.DragMove();
        }
    }
}