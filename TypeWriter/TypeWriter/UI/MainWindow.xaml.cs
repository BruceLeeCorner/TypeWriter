using ImTools;
using Prism.Events;
using Prism.Ioc;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XamlPearls.GlobalKeyShorts;
using Xceed.Wpf.Themes.MaterialDesign;

namespace TypeWriter.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _audioFolder;
        private string _audioPath;
        private AudioPlayMode _playMode;
        private IEventAggregator eg;
        private ShowTypeBoxEvent showTypeBoxEvent;
        private HideTypeBoxEvent hideTypeBoxEvent;
        private AudioControlTypeChangedEvent audioControlTypeChangedEvent;
        private AudioPlayModeChangedEvent audioPlayModeChangedEvent;
        private AudioSelected audioSelectedEvent;

        public MainWindow()
        {
            InitializeComponent();

            eg = App.Instance.Container.Resolve<IEventAggregator>();
            showTypeBoxEvent = eg.GetEvent<ShowTypeBoxEvent>();
            hideTypeBoxEvent = eg.GetEvent<HideTypeBoxEvent>();
            audioControlTypeChangedEvent = eg.GetEvent<AudioControlTypeChangedEvent>();
            audioPlayModeChangedEvent = eg.GetEvent<AudioPlayModeChangedEvent>();
            audioSelectedEvent = eg.GetEvent<AudioSelected>();

            showTypeBoxEvent.Subscribe(() =>
            {
                this.Show();
                TextBlock.Focus();
            });

            hideTypeBoxEvent.Subscribe(() =>
            {
                this.Hide();
            });

            audioControlTypeChangedEvent.Subscribe(arg =>
            {
                var type = (AudioControlType)Enum.Parse(typeof(AudioControlType), arg.ToString());
                if (type == AudioControlType.Next)
                {
                    Next(false);
                }
                else if (type == AudioControlType.Previous)
                {
                    Previous();
                }
                else if (type == AudioControlType.Forward)
                {
                    Forward();
                }
                else if (type == AudioControlType.Back)
                {
                    Back();
                }
                else if (type == AudioControlType.ResetSpeedRatio)
                {
                    ResetSpeedRatio();
                }
                else if (type == AudioControlType.IncrementSpeedRatio)
                {
                    IncreaseSpeedRatio();
                }
                else if (type == AudioControlType.DecrementSpeedRatio)
                {
                    DecreaseSpeedRatio();
                }
            });

            audioPlayModeChangedEvent.Subscribe(arg =>
            {
                _playMode = (AudioPlayMode)Enum.Parse(typeof(AudioPlayMode), arg.ToString());
            });

            audioSelectedEvent.Subscribe(arg =>
            {
                SelectAudio(arg);
            });
            _playMode = AudioPlayMode.SingleLoop;
            _mediaElement.LoadedBehavior = MediaState.Manual;
            _mediaElement.MediaEnded += _mediaElement_MediaEnded;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var model = new HotKeyModel()
            {
                Name = "Next",
                IsSelectCtrl = true,
                IsSelectAlt = false,
                IsSelectShift = false,
                SelectKey = Keys.Down
            };
            if (!this.RegisterGlobalHotKey(model, (model) => { Next(false); }))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel()
            {
                Name = "Prev",
                IsSelectCtrl = true,
                IsSelectAlt = false,
                IsSelectShift = false,
                SelectKey = Keys.Up
            };

            if (!this.RegisterGlobalHotKey(model, (model) =>
            {
                Previous();
            }))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel()
            {
                Name = "Forward",
                IsSelectCtrl = true,
                IsSelectAlt = false,
                IsSelectShift = false,
                SelectKey = Keys.Right
            };

            if (!this.RegisterGlobalHotKey(model, (model) => { Forward(); }))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }
            model = new HotKeyModel()
            {
                Name = "Back",
                IsSelectCtrl = true,
                IsSelectAlt = false,
                IsSelectShift = false,
                SelectKey = Keys.Left
            };

            if (!this.RegisterGlobalHotKey(model, (model) => { Back(); }))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel()
            {
                Name = "IncSpeed",
                IsSelectCtrl = true,
                IsSelectAlt = false,
                IsSelectShift = false,
                SelectKey = Keys.Oemplus
            };
            if (!this.RegisterGlobalHotKey(model, (model) =>
            {
                IncreaseSpeedRatio();
            }))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel()
            {
                Name = "DecSpeed",
                IsSelectCtrl = true,
                IsSelectAlt = false,
                IsSelectShift = false,
                SelectKey = Keys.OemMinus
            };
            if (!this.RegisterGlobalHotKey(model, (model) =>
            {
                DecreaseSpeedRatio();
            }))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }
            model = new HotKeyModel()
            {
                Name = "RstSpeed",
                IsSelectCtrl = false,
                IsSelectAlt = true,
                IsSelectShift = false,
                SelectKey = Keys.R
            };
            if (!this.RegisterGlobalHotKey(model, (model) =>
            {
                ResetSpeedRatio();
            }))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }
        }

        private void _mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            switch (_playMode)
            {
                case AudioPlayMode.SingleLoop:
                    _mediaElement.Position = TimeSpan.Zero;
                    _mediaElement.Play();
                    break;

                case AudioPlayMode.ListLoop:
                    Next(true);
                    break;

                case AudioPlayMode.OrderPlay:
                    Next(true);
                    break;

                case AudioPlayMode.RandomPlay:
                    var files = Directory.GetFiles(_audioFolder, "*.mp3", SearchOption.TopDirectoryOnly).Order().ToArray();
                    if (files.Length > 0)
                    {
                        _mediaElement.Source = new Uri(files[Random.Shared.Next(0, files.Length)]);
                        _mediaElement.Position = TimeSpan.Zero;
                        _mediaElement.Play();
                    }
                    break;

                default:
                    break;
            }
        }

        public void SelectAudio(string path)
        {
            _audioPath = path;
            _audioFolder = Path.GetDirectoryName(path);
            _mediaElement.Source = new Uri(path, UriKind.Absolute);
            _mediaElement.Volume = 1;
            _mediaElement.Play();
        }

        public void IncreaseSpeedRatio()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                if (_mediaElement.SpeedRatio < 3)
                {
                    _mediaElement.Pause();
                    _mediaElement.SpeedRatio += 0.05;
                    _mediaElement.Play();
                }
            }
        }

        public void DecreaseSpeedRatio()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                if (_mediaElement.SpeedRatio > 0.3)
                {
                    _mediaElement.Pause();
                    _mediaElement.SpeedRatio -= 0.05;
                    _mediaElement.Play();
                }
            }
        }

        public void ResetSpeedRatio()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.Pause();
                _mediaElement.SpeedRatio = 1.0;
                _mediaElement.Play();
            }
        }

        public void Next(bool isMediaEnded) // 区分是手动播放下一曲还是自动播放完毕开始下一曲
        {
            if (string.IsNullOrWhiteSpace(_audioFolder) || !Path.Exists(_audioFolder))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"{_audioFolder} doesn't exist.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
                return;
            }

            var files = Directory.GetFiles(_audioFolder, "*.mp3", SearchOption.TopDirectoryOnly).Order().ToArray();
            if (files.Length == 0)
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"{_audioFolder} is empty.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
                return;
            }

            string next = _audioPath;
            bool end = false;

            switch (_playMode)
            {
                case AudioPlayMode.ListLoop:
                    int currIndex = files.IndexOf(item => item == _audioPath);
                    next = currIndex == -1 || currIndex >= files.Length - 1 ? files[0] : files[currIndex + 1];
                    break;

                case AudioPlayMode.SingleLoop:
                    if (isMediaEnded)
                    {
                        next = _audioPath;
                    }
                    else
                    {
                        currIndex = files.IndexOf(item => item == _audioPath);
                        if (currIndex == -1 || currIndex >= files.Length - 1)
                        {
                            end = true;
                        }
                        else
                        {
                            next = files[currIndex + 1];
                        }
                    }
                    break;

                case AudioPlayMode.OrderPlay:
                    currIndex = files.IndexOf(item => item == _audioPath);
                    if (currIndex == -1 || currIndex >= files.Length - 1)
                    {
                        end = true;
                    }
                    else
                    {
                        next = files[currIndex + 1];
                    }
                    break;

                case AudioPlayMode.RandomPlay:
                    next = files[Random.Shared.Next(0, files.Length)];
                    break;
            }

            _audioPath = next;
            _audioFolder = Path.GetDirectoryName(next);

            if (end)
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), "All the audios have been played.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                return;
            }

            _mediaElement.Source = new Uri(next);
            _mediaElement.Position = TimeSpan.Zero;
            _mediaElement.Play();
        }

        public void Previous()
        {
            if (string.IsNullOrWhiteSpace(_audioFolder) || !Path.Exists(_audioFolder))
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"{_audioFolder} doesn't exist.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
                return;
            }

            var files = Directory.GetFiles(_audioFolder, "*.mp3", SearchOption.TopDirectoryOnly).Order().ToArray();
            if (files.Length == 0)
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"{_audioFolder} is empty.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
                return;
            }

            string prev = _audioPath;
            bool end = false;

            switch (_playMode)
            {
                case AudioPlayMode.ListLoop:
                    int currIndex = files.IndexOf(item => item == _audioPath);
                    if (currIndex == -1)
                    {
                        prev = files[0];
                    }
                    else if (currIndex == 0)
                    {
                        prev = files[files.Length - 1];
                    }
                    break;

                case AudioPlayMode.SingleLoop:
                case AudioPlayMode.OrderPlay:
                    currIndex = files.IndexOf(item => item == _audioPath);
                    if (currIndex == -1 || currIndex == 0)
                    {
                        end = true;
                    }
                    else
                    {
                        prev = files[currIndex - 1];
                    }
                    break;

                case AudioPlayMode.RandomPlay:
                    prev = files[Random.Shared.Next(0, files.Length)];
                    break;
            }

            _audioPath = prev;
            _audioFolder = Path.GetDirectoryName(prev);

            if (end)
            {
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), "All the audios have been played.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                return;
            }

            _mediaElement.Source = new Uri(prev);
            _mediaElement.Position = TimeSpan.Zero;
            _mediaElement.Play();
        }

        public void Forward()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.Pause();

                var total = _mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                var pos = _mediaElement.Position.TotalMilliseconds;

                if (total - pos > 5000)
                {
                    _mediaElement.Position = TimeSpan.FromMilliseconds(pos + 5000);
                    _mediaElement.Play();
                }
                else
                {
                    _mediaElement.Position = TimeSpan.Zero;
                    _mediaElement.Play();
                }
            }
        }

        public void Back()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.Pause();

                var total = _mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                var pos = _mediaElement.Position.TotalMilliseconds;

                if (pos > 5000)
                {
                    _mediaElement.Position = TimeSpan.FromMilliseconds(pos - 5000);
                    _mediaElement.Play();
                }
                else
                {
                    _mediaElement.Position = TimeSpan.Zero;
                    _mediaElement.Play();
                }
            }
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

        private void Self_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.Hide();
        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }
    }
}