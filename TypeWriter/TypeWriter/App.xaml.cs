using Hardcodet.Wpf.TaskbarNotification;
using Prism.Events;
using Prism.Ioc;
using System.Windows;
using TypeWriter.UI;

namespace TypeWriter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static App Instance => (Application.Current as App)!;
        public TaskbarIcon TrayIcon => (TaskbarIcon)(this.FindResource("TaskbarIcon"));

        protected override Window? CreateShell()
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
            var trayIcon = FindResource("TaskbarIcon") as TaskbarIcon; // 必须要实例化一下资源，才能激发托盘图标
            trayIcon!.DataContext = new TaskbarIconViewModel(Container.Resolve<IEventAggregator>(), Container.Resolve<AppConfigSource>(), Container.Resolve<SentenceSource>());
            var main = new MainWindow();
            this.MainWindow = main;
            main.Show();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterService(containerRegistry);
            RegisterViewModel(containerRegistry);
        }

        private void RegisterService(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<AppConfigSource>();
            containerRegistry.RegisterSingleton<SentenceSource>();
        }

        private void RegisterViewModel(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MainWindowViewModel>();
        }

        public bool VerifyLicense()
        {
            if (DateTime.Now.Date < new DateTime(2025, 12, 25))
            {
                return true;
            }
            return false;
        }
    }
}