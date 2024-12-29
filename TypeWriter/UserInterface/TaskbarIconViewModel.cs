using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media;
using WpfColorFontDialog;

namespace TypeWriter.UserInterface
{
    internal class TaskbarIconViewModel : BindableBase
    {
        private readonly AppConfigSource _appConfigSource;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly SentenceSource _sentenceSource;
        private readonly WordSource _wordSource;
        private Color _backColor;
        private string _textFilePath;
        private int _typeBoxHeight;
        private int _typeBoxWidth;

        public TaskbarIconViewModel(IEventAggregator eventAggregator, IDialogService dialogService, AppConfigSource appConfigSource, SentenceSource sentenceSource, WordSource wordSource)
        {
            _eventAggregator = eventAggregator;
            _appConfigSource = appConfigSource;
            _sentenceSource = sentenceSource;
            _dialogService = dialogService;
            _wordSource = wordSource;
            _backColor = _appConfigSource.GetConfig().BackColor;
            _typeBoxHeight = _appConfigSource.GetConfig().TypeBoxHeight;
            _typeBoxWidth = _appConfigSource.GetConfig().TypeBoxWidth;

            ObservableLearnWordOption = new ObservableLearnWordOption()
            {
                BoxWidth = appConfigSource.GetConfig().LearnWordOption.BoxWidth,
                BoxHeight = appConfigSource.GetConfig().LearnWordOption.BoxHeight,
                BackColor = appConfigSource.GetConfig().LearnWordOption.BackColor,
                FontInfo = appConfigSource.GetConfig().LearnWordOption.FontInfo,
                Accent = appConfigSource.GetConfig().LearnWordOption.Accent,
            };

            ObservableLearnWordOption.PropertyChanged += (s, e) =>
            {
                var s1 = s as ObservableLearnWordOption;

                if (e.PropertyName == nameof(ObservableLearnWordOption.BoxWidth))
                {
                    _appConfigSource.GetConfig().LearnWordOption.BoxWidth = (int)typeof(ObservableLearnWordOption).GetProperty(e.PropertyName).GetValue(s1);
                }
                if (e.PropertyName == nameof(ObservableLearnWordOption.BoxHeight))
                {
                    _appConfigSource.GetConfig().LearnWordOption.BoxHeight = (int)typeof(ObservableLearnWordOption).GetProperty(e.PropertyName).GetValue(s1);
                }
                if (e.PropertyName == nameof(ObservableLearnWordOption.BackColor))
                {
                    _appConfigSource.GetConfig().LearnWordOption.BackColor = (Color)typeof(ObservableLearnWordOption).GetProperty(e.PropertyName).GetValue(s1);
                }
                if (e.PropertyName == nameof(ObservableLearnWordOption.FontInfo))
                {
                    _appConfigSource.GetConfig().LearnWordOption.FontInfo = (FontInfo)typeof(ObservableLearnWordOption).GetProperty(e.PropertyName).GetValue(s1);
                }
                if (e.PropertyName == nameof(ObservableLearnWordOption.Accent))
                {
                    _appConfigSource.GetConfig().LearnWordOption.Accent = (Accent)typeof(ObservableLearnWordOption).GetProperty(e.PropertyName).GetValue(s1);
                }
                _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
            };

            ShowTypeBoxCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<ShowTypeBoxEvent>().Publish();
            });
        }

        public Color BackColor
        {
            get => _backColor;
            set
            {
                if (SetProperty(ref _backColor, value))
                {
                    _appConfigSource.GetConfig().BackColor = value;
                    _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                    _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
                }
            }
        }

        public ObservableLearnWordOption ObservableLearnWordOption { get; set; }
        public DelegateCommand ShowTypeBoxCommand { get; }

        public int TypeBoxHeight
        {
            get => _typeBoxHeight;
            set
            {
                if (SetProperty(ref _typeBoxHeight, value))
                {
                    _appConfigSource.GetConfig().TypeBoxHeight = value;
                    _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                    _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
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
                    _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
                }
            }
        }

        public void ChangeAccent(object sender, RoutedEventArgs args)
        {
            var tag = ((args.OriginalSource as FrameworkElement).Tag as string);

            if (string.Equals(tag, "UK", StringComparison.OrdinalIgnoreCase))
            {
                ObservableLearnWordOption.Accent = Accent.UK;
                _appConfigSource.GetConfig().LearnWordOption.Accent = Accent.UK;
            }
            else if (string.Equals(tag, "US", StringComparison.OrdinalIgnoreCase))
            {
                ObservableLearnWordOption.Accent = Accent.US;
                _appConfigSource.GetConfig().LearnWordOption.Accent = Accent.US;
            }
            _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
            _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
        }

        public void ChangePlayMode(Object sender, RoutedEventArgs args)
        {
            var tag = (args.OriginalSource as FrameworkElement).Tag;
            if (tag != null)
            {
                if (tag.ToString() == "single")
                {
                    _eventAggregator.GetEvent<AudioPlayModeChangedEvent>().Publish(AudioPlayMode.SingleLoop);
                }
                else if (tag.ToString() == "list")
                {
                    _eventAggregator.GetEvent<AudioPlayModeChangedEvent>().Publish(AudioPlayMode.ListLoop);
                }
                else if (tag.ToString() == "order")
                {
                    _eventAggregator.GetEvent<AudioPlayModeChangedEvent>().Publish(AudioPlayMode.OrderPlay);
                }
                else if (tag.ToString() == "random")
                {
                    _eventAggregator.GetEvent<AudioPlayModeChangedEvent>().Publish(AudioPlayMode.RandomPlay);
                }
            }
        }

        public void Exit()
        {
            App.Instance.MainWindow!.Close();
        }

        public void OpenTextFolder()
        {
            Window w = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = Brushes.Transparent
            };
            w.Show();
            w.WindowState = WindowState.Minimized;
            if (!string.IsNullOrWhiteSpace(_textFilePath))
            {
                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(_textFilePath));
            }
            else
            {
                MessageBox.Show("You haven't selected the file.", nameof(TypeWriter), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            w.Close();
        }

        public void SelectAudio()
        {
            Window w = ShowBackWindow();

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio|*.mp3",
                InitialDirectory = Environment.CurrentDirectory,
                RestoreDirectory = true,
                Multiselect = false,
                DefaultExt = ".mp3"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _eventAggregator.GetEvent<AudioSelected>().Publish(openFileDialog.FileName);
            }

            w.Close();
        }

        public void SelectFile()
        {
            Window w = ShowBackWindow();

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text documents (.txt)|*.txt",
                InitialDirectory = Environment.CurrentDirectory,
                RestoreDirectory = true,
                Multiselect = false,
                DefaultExt = ".txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _textFilePath = openFileDialog.FileName;
                _sentenceSource.LoadText(_textFilePath);
                _eventAggregator.GetEvent<NewFileLoadedEvent>().Publish();
            }

            w.Close();
        }

        public void SelectWordFile()
        {
            Window w = ShowBackWindow();

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text documents (.txt)|*.txt",
                InitialDirectory = Environment.CurrentDirectory,
                RestoreDirectory = true,
                Multiselect = false,
                DefaultExt = ".txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _wordSource.LoadWords(openFileDialog.FileName);
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
                _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
            }
        }

        public void SetLearnWordFont()
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
                _appConfigSource.GetConfig().LearnWordOption.FontInfo = dialog.Font;
                _appConfigSource.SaveConfig(_appConfigSource.GetConfig());
                _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
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
                _eventAggregator.GetEvent<AppConfigChangedEvent>().Publish();
            }
        }

        public void ShowWordBox()
        {
            foreach (var window in App.Instance.Windows)
            {
                var w = window as Window;
                if (w != null && w.DataContext != null && w.DataContext.GetType() == typeof(LearnWordViewModel))
                {
                    w.Show();
                    return;
                }
            }
            _dialogService.Show("learn_word");
        }

        // 不先New Window Show，文件选择对话框会闪退。这应该是Hardcodet.NotifyIcon.Wpf的bug.
        private Window ShowBackWindow()
        {
            Window w = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            w.Show();
            return w;
        }
    }
}