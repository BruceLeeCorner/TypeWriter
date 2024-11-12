using CommunityToolkit.Mvvm.Messaging;
using Prism.Events;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeWriter.UI
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly AppConfigSource _appConfigSource;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessenger _messenger;
        private readonly SentenceSource _sentenceSource;
        private Color _backColor;

        private Color _toTypeFontColor;
        private FontFamily _toTypeFontFamily;
        private double _toTypeFontSize;
        private FontStretch _toTypeFontStretch;
        private FontStyle _toTypeFontStyle;
        private FontWeight _toTypeFontWeight;
        private string _toTypeString;
        private int _typeBoxHeight;
        private int _typeBoxWidth;
        private Color _typedColor;
        private FontFamily _typedFontFamily;
        private double _typedFontSize;
        private FontStretch _typedFontStretch;
        private FontStyle _typedFontStyle;
        private FontWeight _typedFontWeight;
        private string _typedString;

        public MainWindowViewModel(AppConfigSource appConfigSource, SentenceSource sentenceSource, IEventAggregator eventAggregator, IMessenger messenger)
        {
            _appConfigSource = appConfigSource;
            _sentenceSource = sentenceSource;
            _eventAggregator = eventAggregator;
            _messenger = messenger;

            _typeBoxHeight = appConfigSource.GetConfig().TypeBoxHeight;
            _typeBoxWidth = appConfigSource.GetConfig().TypeBoxWidth;

            _backColor = appConfigSource.GetConfig().BackColor;

            _toTypeFontColor = appConfigSource.GetConfig().ToTypeFont.BrushColor.Color;
            _toTypeFontFamily = appConfigSource.GetConfig().ToTypeFont.Family;
            _toTypeFontSize = appConfigSource.GetConfig().ToTypeFont.Size;
            _toTypeFontStretch = appConfigSource.GetConfig().ToTypeFont.Stretch;
            _toTypeFontStyle = appConfigSource.GetConfig().ToTypeFont.Style;
            _toTypeFontWeight = appConfigSource.GetConfig().ToTypeFont.Weight;

            _typedColor = appConfigSource.GetConfig().TypedFont.BrushColor.Color;
            _typedFontFamily = appConfigSource.GetConfig().TypedFont.Family;
            _typedFontSize = appConfigSource.GetConfig().TypedFont.Size;
            _typedFontStretch = appConfigSource.GetConfig().TypedFont.Stretch;
            _typedFontStyle = appConfigSource.GetConfig().TypedFont.Style;
            _typedFontWeight = appConfigSource.GetConfig().TypedFont.Weight;

            _sentenceSource.CharTyped += _sentenceSource_CharTyped;

            _messenger.Register<string, string>(this, "app_config", (o, m) =>
            {
                TypeBoxHeight = appConfigSource.GetConfig().TypeBoxHeight;
                TypeBoxWidth = appConfigSource.GetConfig().TypeBoxWidth;

                BackColor = appConfigSource.GetConfig().BackColor;

                ToTypeFontColor = appConfigSource.GetConfig().ToTypeFont.BrushColor.Color;
                ToTypeFontFamily = appConfigSource.GetConfig().ToTypeFont.Family;
                ToTypeFontSize = appConfigSource.GetConfig().ToTypeFont.Size;
                ToTypeFontStretch = appConfigSource.GetConfig().ToTypeFont.Stretch;
                ToTypeFontStyle = appConfigSource.GetConfig().ToTypeFont.Style;
                ToTypeFontWeight = appConfigSource.GetConfig().ToTypeFont.Weight;

                TypedFontColor = appConfigSource.GetConfig().TypedFont.BrushColor.Color;
                TypedFontFamily = appConfigSource.GetConfig().TypedFont.Family;
                TypedFontSize = appConfigSource.GetConfig().TypedFont.Size;
                TypedFontStretch = appConfigSource.GetConfig().TypedFont.Stretch;
                TypedFontStyle = appConfigSource.GetConfig().TypedFont.Style;
                TypedFontWeight = appConfigSource.GetConfig().TypedFont.Weight;
            });

            _messenger.Register<string, string>(this, "load_file", (o, m) =>
            {
                TypedString = string.Empty;
                ToTypeString = string.Empty;
            });

            //_eventAggregator.GetEvent<AppConfigChangedEvent>().Subscribe(() =>
            //{
            //    TypeBoxHeight = appConfigSource.GetConfig().TypeBoxHeight;
            //    TypeBoxWidth = appConfigSource.GetConfig().TypeBoxWidth;

            //    BackColor = appConfigSource.GetConfig().BackColor;

            //    ToTypeFontColor = appConfigSource.GetConfig().ToTypeFont.BrushColor.Color;
            //    ToTypeFontFamily = appConfigSource.GetConfig().ToTypeFont.Family;
            //    ToTypeFontSize = appConfigSource.GetConfig().ToTypeFont.Size;
            //    ToTypeFontStretch = appConfigSource.GetConfig().ToTypeFont.Stretch;
            //    ToTypeFontStyle = appConfigSource.GetConfig().ToTypeFont.Style;
            //    ToTypeFontWeight = appConfigSource.GetConfig().ToTypeFont.Weight;

            //    TypedFontColor = appConfigSource.GetConfig().TypedFont.BrushColor.Color;
            //    TypedFontFamily = appConfigSource.GetConfig().TypedFont.Family;
            //    TypedFontSize = appConfigSource.GetConfig().TypedFont.Size;
            //    TypedFontStretch = appConfigSource.GetConfig().TypedFont.Stretch;
            //    TypedFontStyle = appConfigSource.GetConfig().TypedFont.Style;
            //    TypedFontWeight = appConfigSource.GetConfig().TypedFont.Weight;
            //}, ThreadOption.UIThread);

            //_eventAggregator.GetEvent<LoadFileEvent>().Subscribe(() =>
            //{
            //    TypedString = string.Empty;
            //    ToTypeString = string.Empty;
            //}, ThreadOption.UIThread);

        }

        private void _sentenceSource_CharTyped((string typedString, string toTypeString) obj)
        {
            TypedString = obj.typedString;
            ToTypeString = obj.toTypeString;
        }

        #region Properties

        public Color BackColor
        {
            get => _backColor;
            set => SetProperty(ref _backColor, value);
        }

        public Color ToTypeFontColor
        {
            get => _toTypeFontColor;
            set => SetProperty(ref _toTypeFontColor, value);
        }

        public FontFamily ToTypeFontFamily
        {
            get => _toTypeFontFamily;
            set => SetProperty(ref _toTypeFontFamily, value);
        }

        public double ToTypeFontSize
        {
            get => _toTypeFontSize;
            set => SetProperty(ref _toTypeFontSize, value);
        }

        public FontStretch ToTypeFontStretch
        {
            get => _toTypeFontStretch;
            set => SetProperty(ref _toTypeFontStretch, value);
        }

        public FontStyle ToTypeFontStyle
        {
            get => _toTypeFontStyle;
            set => SetProperty(ref _toTypeFontStyle, value);
        }

        public FontWeight ToTypeFontWeight
        {
            get => _toTypeFontWeight;
            set => SetProperty(ref _toTypeFontWeight, value);
        }

        public string ToTypeString
        {
            get => _toTypeString;
            set => SetProperty(ref _toTypeString, value);
        }

        public int TypeBoxHeight
        {
            get { return _typeBoxHeight; }
            set { SetProperty(ref _typeBoxHeight, value); }
        }

        public int TypeBoxWidth
        {
            get { return _typeBoxWidth; }
            set { SetProperty(ref _typeBoxWidth, value); }
        }

        public Color TypedFontColor
        {
            get => _typedColor;
            set => SetProperty(ref _typedColor, value);
        }

        public FontFamily TypedFontFamily
        {
            get => _typedFontFamily;
            set => SetProperty(ref _typedFontFamily, value);
        }

        public double TypedFontSize
        {
            get => _typedFontSize;
            set => SetProperty(ref _typedFontSize, value);
        }

        public FontStretch TypedFontStretch
        {
            get => _typedFontStretch;
            set => SetProperty(ref _typedFontStretch, value);
        }

        public FontStyle TypedFontStyle
        {
            get => _typedFontStyle;
            set => SetProperty(ref _typedFontStyle, value);
        }

        public FontWeight TypedFontWeight
        {
            get => _typedFontWeight;
            set => SetProperty(ref _typedFontWeight, value);
        }

        public string TypedString
        {
            get => _typedString;
            set => SetProperty(ref _typedString, value);
        }

        #endregion Properties

        public void TextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text[0] == ' ')
            {
                _messenger.Send("hide","hide");
                return;
            }
            _sentenceSource.OnInputChar(e.Text[0]);
        }

        public void NextSentence()
        {
            _sentenceSource.NextSentence();
        }

        public void PrevSentence()
        {
            _sentenceSource.PrevSentence();
        }
    }
}