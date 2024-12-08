using System.Windows.Media;
using WpfColorFontDialog;

namespace TypeWriter
{
    public class AppConfig
    {
        public int TypeBoxWidth { get; set; }
        public int TypeBoxHeight { get; set; }
        public FontInfo TypedFont { get; set; }
        public FontInfo ToTypeFont { get; set; }
        public Color BackColor { get; set; }

    }
}