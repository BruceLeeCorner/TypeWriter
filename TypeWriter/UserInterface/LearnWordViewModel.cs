using CsvHelper;
using NAudio.Wave;
using Nito.AsyncEx;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeWriter.UserInterface
{
    internal class LearnWordViewModel : BindableBase, IDialogAware
    {
        private readonly AppConfigSource _appConfigSource;
        private readonly IEventAggregator _eventAggregator;
        private readonly AsyncLock _mutex = new AsyncLock();
        private readonly Dictionary<string, string> _phonetics;
        private readonly Timer _timer;
        private readonly Dictionary<string, (byte[] us, byte[] uk)> _wordAudioCache;
        private readonly WordSource _wordSource;
        private Accent _accent;
        private Color _backColor;
        private int _boxHeight;
        private int _boxWidth;
        private Color _fontColor;
        private FontFamily _fontFamily;
        private double _fontSize;
        private FontStretch _fontStretch;
        private FontStyle _fontStyle;
        private FontWeight _fontWeight;
        private string _word;

        #region Commands

        public DelegateCommand<KeyEventArgs> KeyDownCommand { get; set; }
        public AsyncDelegateCommand NextCommand { get; set; }
        public AsyncDelegateCommand PrevCommand { get; set; }

        #endregion Commands

        public LearnWordViewModel(WordSource wordSource, AppConfigSource appConfigSource, IEventAggregator eventAggregator)
        {
            NextCommand = new AsyncDelegateCommand(Next).Catch((ex) => { });
            PrevCommand = new AsyncDelegateCommand(Previous).Catch((ex) => { });
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(KeyDown);
            _wordSource = wordSource;
            _phonetics = new Dictionary<string, string>();
            _wordAudioCache = new Dictionary<string, (byte[] us, byte[] uk)>();
            _appConfigSource = appConfigSource;
            _eventAggregator = eventAggregator;
            _backColor = _appConfigSource.GetConfig().LearnWordOption.BackColor;
            _boxHeight = _appConfigSource.GetConfig().LearnWordOption.BoxHeight;
            _boxWidth = _appConfigSource.GetConfig().LearnWordOption.BoxWidth;
            _fontColor = _appConfigSource.GetConfig().LearnWordOption.FontInfo.BrushColor.Color;
            _fontSize = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Size;
            _fontFamily = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Family;
            _fontStretch = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Stretch;
            _fontStyle = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Style;
            _fontWeight = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Weight;
            _accent = _appConfigSource.GetConfig().LearnWordOption.Accent;

            _eventAggregator.GetEvent<AppConfigChangedEvent>().Subscribe(() =>
            {
                BackColor = _appConfigSource.GetConfig().LearnWordOption.BackColor;
                BoxHeight = _appConfigSource.GetConfig().LearnWordOption.BoxHeight;
                BoxWidth = _appConfigSource.GetConfig().LearnWordOption.BoxWidth;
                FontColor = _appConfigSource.GetConfig().LearnWordOption.FontInfo.BrushColor.Color;
                FontSize = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Size;
                FontFamily = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Family;
                FontStretch = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Stretch;
                FontStyle = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Style;
                FontWeight = _appConfigSource.GetConfig().LearnWordOption.FontInfo.Weight;
                Accent = _appConfigSource.GetConfig().LearnWordOption.Accent;
            });

            LoadPhonetic();

            _timer = new Timer((state) =>
            {
                PlayWordAudio(Word).GetAwaiter().GetResult();
                _timer.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
            });
            _timer.Change(TimeSpan.FromSeconds(0), Timeout.InfiniteTimeSpan);
        }

        public event Action<IDialogResult> RequestClose;

        public Accent Accent
        {
            get => _accent;
            set
            {
                SetProperty(ref _accent, value);
            }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { SetProperty(ref _backColor, value); }
        }

        public int BoxHeight
        {
            get { return _boxHeight; }
            set { SetProperty(ref _boxHeight, value); }
        }

        public int BoxWidth
        {
            get { return _boxWidth; }
            set { SetProperty(ref _boxWidth, value); }
        }

        public Color FontColor
        {
            get => _fontColor;
            set => SetProperty(ref _fontColor, value);
        }

        public FontFamily FontFamily
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        public FontStretch FontStretch
        {
            get => _fontStretch;
            set => SetProperty(ref _fontStretch, value);
        }

        public FontStyle FontStyle
        {
            get => _fontStyle;
            set => SetProperty(ref _fontStyle, value);
        }

        public FontWeight FontWeight
        {
            get => _fontWeight;
            set => SetProperty(ref _fontWeight, value);
        }

        public string Phonetic
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Word) || !_phonetics.ContainsKey(Word.ToLower()))
                {
                    return string.Empty;
                }
                return _phonetics[Word.ToLower()];
            }
        }

        DialogCloseListener IDialogAware.RequestClose { get; }

        public string Title => string.Empty;

        public string Word
        {
            get { return _word; }
            set { SetProperty(ref _word, value); }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        private void KeyDown(KeyEventArgs args)
        {
            TextBox textBox = args.Source as TextBox;
            textBox.Focus();
            if (args.Key == Key.Enter)
            {
                textBox.Clear();
            }
            else if (args.Key == Key.Up)
            {
                _ = Previous();
            }
            else if (args.Key == Key.Down)
            {
                _ = Next();
            }
            else if (args.Key == Key.Left)
            {
                Accent = Accent.US;
            }
            else if (args.Key == Key.Right)
            {
                Accent = Accent.UK;
            }
            else if(args.Key == Key.Space)
            {
                var parent = LogicalTreeHelper.GetParent(textBox);
                while (parent is not Window)
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }
                (parent as Window).Hide();
            }
        }

        private void LoadPhonetic()
        {
            _phonetics.Clear();
            using var reader = new StreamReader("word.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                string word = csv.GetField("vc_vocabulary");
                string phoneticUS = csv.GetField("vc_phonetic_us");
                string phoneticUK = csv.GetField("vc_phonetic_uk");
                _phonetics[word.ToLower()] = phoneticUS + " " + phoneticUK;
            }
        }

        private async Task Next()
        {
            _wordSource.Next();
            Word = _wordSource.Word;
            RaisePropertyChanged(nameof(Phonetic));
            await PlayWordAudio(Word);
        }

        private async Task PlayWordAudio(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return;
            }

            using (await _mutex.LockAsync())
            {
                byte[] audioBytes;
                var accent = _accent;
                if (_wordAudioCache.ContainsKey(word))
                {
                    if (accent == Accent.UK)
                    {
                        audioBytes = _wordAudioCache[word].uk;
                    }
                    else
                    {
                        audioBytes = _wordAudioCache[word].us;
                    }
                }
                else
                {
                    using HttpClient client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(1);
                    var ukAudio = await client.GetByteArrayAsync($"https://dict.youdao.com/dictvoice?audio={word}&type={1}");
                    var usAudio = await client.GetByteArrayAsync($"https://dict.youdao.com/dictvoice?audio={word}&type={2}");
                    _wordAudioCache[word] = (usAudio, ukAudio);
                    audioBytes = accent == Accent.UK ? ukAudio : usAudio;
                }

                using (var ms = new MemoryStream(audioBytes))
                using (var media = new StreamMediaFoundationReader(ms))
                using (var waveOut = new WaveOutEvent())
                {
                    waveOut.Init(media);
                    waveOut.Play();
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100);
                    }
                }
            }
        }

        private async Task Previous()
        {
            _wordSource.Previous();
            Word = _wordSource.Word;
            RaisePropertyChanged(nameof(Phonetic));

            await PlayWordAudio(Word);
        }
    }
}