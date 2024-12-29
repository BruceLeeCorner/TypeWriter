using DryIoc.ImTools;
using NLog;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XamlPearls.Shortcuts;

namespace TypeWriter.UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private string _audioFolder;
        private string _audioPath;
        private AudioPlayMode _playMode;
        private AudioControlTypeChangedEvent audioControlTypeChangedEvent;
        private AudioPlayModeChangedEvent audioPlayModeChangedEvent;
        private AudioSelected audioSelectedEvent;
        private IEventAggregator eg;
        private HideTypeBoxEvent hideTypeBoxEvent;
        private ShowTypeBoxEvent showTypeBoxEvent;
        private bool togglePlayStatus = false;

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
                    Forward(3500);
                }
                else if (type == AudioControlType.Back)
                {
                    Back(3500);
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

        public void Back(int milliseconds)
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.Pause();

                var total = _mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                var pos = _mediaElement.Position.TotalMilliseconds;

                if (pos > milliseconds)
                {
                    _mediaElement.Position = TimeSpan.FromMilliseconds(pos - milliseconds);
                    _mediaElement.Play();
                }
                else
                {
                    _mediaElement.Position = TimeSpan.Zero;
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
                    _mediaElement.SpeedRatio -= 0.05;
                    Back(200);
                }
            }
        }

        public void Forward(int milliseconds)
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.Pause();

                var total = _mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                var pos = _mediaElement.Position.TotalMilliseconds;

                if (total - pos > milliseconds)
                {
                    _mediaElement.Position = TimeSpan.FromMilliseconds(pos + milliseconds);
                    _mediaElement.Play();
                }
                else
                {
                    _mediaElement.Position = TimeSpan.Zero;
                    _mediaElement.Play();
                }
            }
        }

        public void IncreaseSpeedRatio()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                if (_mediaElement.SpeedRatio < 3)
                {
                    _mediaElement.SpeedRatio += 0.05;
                    Back(200);
                }
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

        public void Pause()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.Pause();
            }
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

        public void ResetSpeedRatio()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.SpeedRatio = 1.0;
                Back(200);
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

        public void Start()
        {
            if (File.Exists(_audioPath) && _mediaElement.HasAudio)
            {
                _mediaElement.Play();
            }
        }

        public void TogglePlayStatus()
        {
            if (togglePlayStatus)
            {
                Start();
            }
            else
            {
                Pause();
            }
            togglePlayStatus = !togglePlayStatus;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var model = new HotKeyModel("Toggle", true, false, false, false, Keys.Space);
            try
            {
                this.RegisterGlobalHotKey(model, (model) => { TogglePlayStatus(); });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            //var model = new HotKeyModel("Pause", true, false, true, false, Keys.P);
            //try
            //{
            //    this.RegisterGlobalHotKey(model, (model) => { Pause(); });
            //}
            //catch (Exception ex)
            //{
            //    _logger.Warn(ex);
            //    App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            //}

            //model = new HotKeyModel("Start", true, false, true, false, Keys.S);
            //try
            //{
            //    this.RegisterGlobalHotKey(model, (model) => { Start(); });
            //}
            //catch (Exception ex)
            //{
            //    _logger.Warn(ex);
            //    App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            //}

            model = new HotKeyModel("Next", true, false, false, false, Keys.Down);
            try
            {
                this.RegisterGlobalHotKey(model, (model) => { Next(false); });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel("Prev", true, false, false, false, Keys.Up);

            try
            {
                this.RegisterGlobalHotKey(model, (model) =>
                {
                    Previous();
                });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel("Forward", true, false, false, false, Keys.Right);

            try
            {
                this.RegisterGlobalHotKey(model, (model) => { Forward(3500); });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel("Back", true, false, false, false, Keys.Left);

            try
            {
                this.RegisterGlobalHotKey(model, (model) => { Back(3500); });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel("IncSpeed", true, false, false, false, Keys.Oemplus);
            try
            {
                this.RegisterGlobalHotKey(model, (model) =>
                {
                    IncreaseSpeedRatio();
                });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel("DecSpeed", true, false, false, false, Keys.OemMinus);

            try
            {
                this.RegisterGlobalHotKey(model, (model) =>
                {
                    DecreaseSpeedRatio();
                });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                App.Instance.TrayIcon.ShowBalloonTip(nameof(TypeWriter), $"Failed to register {model.Name}.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
            }

            model = new HotKeyModel("RstSpeed", true, false, false, false, Keys.R);

            try
            {
                this.RegisterGlobalHotKey(model, (model) =>
                {
                    ResetSpeedRatio();
                });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
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

        private void Self_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.Hide();
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock.Focus();
        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock.Focus();
            this.DragMove();
        }
    }
}