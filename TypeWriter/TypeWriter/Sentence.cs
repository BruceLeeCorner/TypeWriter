namespace Xueban.TypeWriter
{
    internal class Sentence : ISentence
    {
        private readonly List<string> _words;
        private int _currWordIndex;
        private int _currCharIndex;

        public string CurrWord => _words.Count == 0 ? null : _words[_currWordIndex];

        public Sentence(string? newLine)
        {
            _words = new List<string>();
            if (newLine != null && !string.IsNullOrWhiteSpace(newLine))
            {
                _words.AddRange(newLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()));
            }
        }

        public bool IsEmpty => _words.Count == 0;
        public bool IsEnd => _words.Count == 0 || (_currWordIndex == _words.Count - 1 && _currCharIndex == _words[_currWordIndex].Length - 1);

        public bool RecvInputChar(char @char, out string typed, out string toType, out bool missMatch)
        {
            if (IsEnd)
            {
                toType = string.Empty;
                typed = string.Empty;
                missMatch = false;
                return IsEnd;
            }

            var wordArray = _words.ToArray();

            typed = string.Join(" ", wordArray, 0, _currWordIndex)
                   + " " + _words[_currWordIndex].Take(_currCharIndex + 1);
            toType = wordArray[_currWordIndex].Skip(_currCharIndex + 1) + " " + string.Join(" ", wordArray,
                _currWordIndex + 1, wordArray.Length - _currWordIndex - 1);

            if (string.Equals(@char.ToString(), _words[_currWordIndex][_currCharIndex].ToString(),
                    StringComparison.CurrentCultureIgnoreCase))
            {
                _currCharIndex++;
                if (_currCharIndex == _words[_currWordIndex].Length)
                {
                    _currCharIndex = 0;
                    _currWordIndex++;
                }

                missMatch = false;
                return IsEnd;
            }

            missMatch = true;
            return IsEnd;
            // 彩虹气泡 + Play Sound   missing key
        }
    }
}