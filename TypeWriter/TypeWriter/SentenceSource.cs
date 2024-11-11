using System.IO;

namespace Xueban.TypeWriter
{
    internal class SentenceSource : ISentenceSource
    {
        private readonly List<string> _lines;
        private int _currentLineIndex;

        public SentenceSource()
        {
            _lines = new List<string>();
        }

        public void Load(string path)
        {
            if (Path.GetExtension(path).ToLower() != ".txt")
            {
                throw new ArgumentException("The file extension must be .txt");
            }
            _lines.Clear();
            _currentLineIndex = 0;
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    _lines.Add(line.Trim());
                }
            }
        }

        public bool IsEmpty => _lines.Count == 0;
        public bool OutOfUpperRange => IsEmpty || _currentLineIndex >= _lines.Count;
        public bool OutOfLowerRange => IsEmpty || _currentLineIndex < 0;

        public bool Next()
        {
            _currentLineIndex++;
            if (OutOfUpperRange)
            {
                _currentLineIndex = _lines.Count;
            }
            return OutOfUpperRange;
        }

        public bool Prev()
        {
            _currentLineIndex--;
            if (OutOfLowerRange)
            {
                _currentLineIndex = -1;
            }
            return OutOfLowerRange;
        }

        public string? CurrentLine => _lines.Count == 0 || _currentLineIndex < 0 || _currentLineIndex >= _lines.Count ? null : _lines[_currentLineIndex];
    }
}