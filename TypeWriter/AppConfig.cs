using System.Windows.Media;
using WpfColorFontDialog;

namespace TypeWriter
{
    public enum Accent
    {
        UK = 1,
        US = 2,
    }

    public class AppConfig
    {
        #region Properties

        public Color BackColor { get; set; }
        public LearnWordOption LearnWordOption { get; set; }
        public FontInfo ToTypeFont { get; set; }
        public int TypeBoxHeight { get; set; }
        public int TypeBoxWidth { get; set; }
        public FontInfo TypedFont { get; set; }

        #endregion Properties
    }

    public class LearnWordOption
    {
        #region Properties

        public Accent Accent { get; set; }
        public Color BackColor { get; set; }
        public int BoxHeight { get; set; }
        public int BoxWidth { get; set; }
        public FontInfo FontInfo { get; set; }

        #endregion Properties
    }
}