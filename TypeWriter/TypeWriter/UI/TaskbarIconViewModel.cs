using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Media;
using WpfColorFontDialog;

namespace TypeWriter.UI
{
    internal class TaskbarIconViewModel : BindableBase
    {
        private readonly AppConfigSource _appConfigSource;
        private readonly IEventAggregator _eventAggregator;
        private readonly SentenceSource _sentenceSource;
        private readonly IMessenger _messenger;
        private int _typeBoxHeight;
        private int _typeBoxWidth;
        private Color _backColor;

        public TaskbarIconViewModel(IEventAggregator eventAggregator, AppConfigSource appConfigSource, SentenceSource sentenceSource, IMessenger messenger)
        {
            _eventAggregator = eventAggregator;
            _appConfigSource = appConfigSource;
            _sentenceSource = sentenceSource;
            _messenger = messenger;
            _backColor = _appConfigSource.GetConfig().BackColor;
            _typeBoxHeight = _appConfigSource.GetConfig().TypeBoxHeight;
            _typeBoxWidth = _appConfigSource.GetConfig().TypeBoxWidth;
            ShowTypeBoxCommand = new DelegateCommand(() =>
            {
                _messenger.Send("show_typebox", "show_typebox");
            });
        }

        public DelegateCommand ShowTypeBoxCommand { get;}

        public Color BackColor
        {
            get => _backColor;
            set
            {
                if (SetProperty(ref _backColor, value))
                {
                    _appConfigSource.GetConfig().BackColor = value;
                    _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                    _messenger.Send<string, string>("app_config", "app_config");
                    //_eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
                }
            }
        }

        public int TypeBoxHeight
        {
            get => _typeBoxHeight;
            set
            {
                if (SetProperty(ref _typeBoxHeight, value))
                {
                    _appConfigSource.GetConfig().TypeBoxHeight = value;
                    _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                    _messenger.Send<string, string>("app_config", "app_config");
                    //_eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
                }
            }
        }

        public int TypeBoxWidth
        {
            get => _typeBoxWidth;
            set
            {
                if (SetProperty(ref _typeBoxWidth, value))
                {
                    _appConfigSource.GetConfig().TypeBoxWidth = value;
                    _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                    _messenger.Send<string, string>("app_config", "app_config");
                    //_eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
                }
            }
        }

        public void Exit()
        {
            App.Instance.MainWindow!.Close();
        }

        public void LoadFile()
        {
            // 不先New Window Show，文件选择框会闪退。这应该是.NET8 WPF的bug.
            Window w = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            w.Show();

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text documents (.txt)|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Multiselect = false,
                DefaultExt = ".txt"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                _sentenceSource.LoadText(openFileDialog.FileName);
                _messenger.Send("load_file", "load_file");
                //_eventAggregator.GetEvent<LoadFileEvent>().Publish();
            }

            w.Close();
        }

        public void SetToTypeFont()
        {
            ColorFontDialog dialog = new ColorFontDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // need that mainwindow has been shown.
            dialog.Font = FontInfo.GetControlFont(Application.Current.MainWindow);

            dialog.Font.Family = _appConfigSource.GetConfig().ToTypeFont.Family;
            dialog.Font.Weight = _appConfigSource.GetConfig().ToTypeFont.Weight;
            dialog.Font.Style = _appConfigSource.GetConfig().ToTypeFont.Style;
            dialog.Font.Stretch = _appConfigSource.GetConfig().ToTypeFont.Stretch;
            dialog.Font.Size = _appConfigSource.GetConfig().ToTypeFont.Size;
            dialog.Font.BrushColor = _appConfigSource.GetConfig().ToTypeFont.BrushColor;

            if (dialog.ShowDialog() == true)
            {
                _appConfigSource.GetConfig().ToTypeFont = dialog.Font;
                _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                //_eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
                _messenger.Send<string, string>("app_config", "app_config");
            }
        }

        public void SetTypedFont()
        {
            ColorFontDialog dialog = new ColorFontDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // need that mainwindow has been shown.
            dialog.Font = FontInfo.GetControlFont(Application.Current.MainWindow);

            dialog.Font.Family = _appConfigSource.GetConfig().TypedFont.Family;
            dialog.Font.Weight = _appConfigSource.GetConfig().TypedFont.Weight;
            dialog.Font.Style = _appConfigSource.GetConfig().TypedFont.Style;
            dialog.Font.Stretch = _appConfigSource.GetConfig().TypedFont.Stretch;
            dialog.Font.Size = _appConfigSource.GetConfig().TypedFont.Size;
            dialog.Font.BrushColor = _appConfigSource.GetConfig().TypedFont.BrushColor;

            if (dialog.ShowDialog() == true)
            {
                _appConfigSource.GetConfig().TypedFont = dialog.Font;
                _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                //_eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
                _messenger.Send<string, string>("app_config", "app_config");
            }
        }
    }
}