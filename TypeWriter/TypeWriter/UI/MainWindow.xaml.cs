
using DryIoc.Messages;
using Prism.Events;
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

            App.Instance.Container.Resolve<IEventAggregator>().GetEvent<ShowTypeBoxEvent>().Subscribe(() =>
            {
                this.Show();
                TextBlock.Focus();
            });

            App.Instance.Container.Resolve<IEventAggregator>().GetEvent<HideTypeBox>().Subscribe(() =>
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