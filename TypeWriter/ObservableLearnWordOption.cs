using System.Windows.Media;
using WpfColorFontDialog;

namespace TypeWriter
{
    internal class ObservableLearnWordOption : BindableBase
    {
        #region Fields

        private readonly LearnWordOption learnWordOption;

        #endregion Fields

        #region Public Constructors

        public ObservableLearnWordOption()
        {
            learnWordOption = new LearnWordOption();
        }

        #endregion Public Constructors

        #region Properties

        public Accent Accent
        {
            get
            {
                return learnWordOption.Accent;
            }
            set
            {
                learnWordOption.Accent = value;
                RaisePropertyChanged();
            }
        }

        public Color BackColor
        {
            get
            {
                return learnWordOption.BackColor;
            }
            set
            {
                learnWordOption.BackColor = value;
                RaisePropertyChanged();
            }
        }

        public int BoxHeight
        {
            get
            {
                return learnWordOption.BoxHeight;
            }
            set
            {
                learnWordOption.BoxHeight = value;
                RaisePropertyChanged();
            }
        }

        public int BoxWidth
        {
            get
            {
                return learnWordOption.BoxWidth;
            }
            set
            {
                learnWordOption.BoxWidth = value;
                RaisePropertyChanged();
            }
        }

        public FontInfo FontInfo
        {
            get
            {
                return learnWordOption.FontInfo;
            }
            set
            {
                learnWordOption.FontInfo = value;
                RaisePropertyChanged();
            }
        }

        #endregion Properties
    }
}