using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using TypeWriter.UserInterface;

namespace TypeWriter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        public static App Instance => (Application.Current as App)!;
        public TaskbarIcon TrayIcon => (TaskbarIcon)(this.FindResource("TaskbarIcon"));

        public bool VerifyLicense()
        {
            if (DateTime.Now.Date < new DateTime(2025, 12, 31))
            {
                return true;
            }
            return false;
        }

        protected override Window CreateShell()
        {
            return null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Xceed.Wpf.Toolkit.Licenser.LicenseKey = "WTK46-P1SP9-RR9GS-0RHA";
            if (!VerifyLicense())
            {
                this.Shutdown();
            }
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            var trayIcon = FindResource("TaskbarIcon") as TaskbarIcon; // 必须要实例化一下资源，才能激发托盘图标
            trayIcon!.DataContext = new TaskbarIconViewModel(Container.Resolve<IEventAggregator>(), Container.Resolve<IDialogService>(), Container.Resolve<AppConfigSource>(), Container.Resolve<SentenceSource>(), Container.Resolve<WordSource>());
            var main = new MainWindow();
            this.MainWindow = main;
            main.Show();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterService(containerRegistry);
            RegisterViewModel(containerRegistry);
            containerRegistry.RegisterDialog<LearnWordView>("learn_word");
            containerRegistry.RegisterDialogWindow<Window1>();
        }

        private void RegisterService(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<AppConfigSource>();
            containerRegistry.RegisterSingleton<SentenceSource>();
            containerRegistry.RegisterSingleton<WordSource>();
        }

        private void RegisterViewModel(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MainWindowViewModel>();
            containerRegistry.Register<LearnWordViewModel>();
        }

    }
}