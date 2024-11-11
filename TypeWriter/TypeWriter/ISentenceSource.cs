namespace Xueban.TypeWriter
{
    internal interface ISentenceSource
    {
        void Load(string path);

        bool IsEmpty { get; }
        bool OutOfUpperRange { get; }
        bool OutOfLowerRange { get; }

        bool Next();

        bool Prev();

        string? CurrentLine { get; }
    }
}