namespace Zmeika.App.Rendering;

internal sealed class LargeFont
{
    private const char PatternBlock = '#';

    private readonly IReadOnlyDictionary<char, string[]> _letters;

    public LargeFont(int height, int letterSpacing, int wordSpacing, IReadOnlyDictionary<char, string[]> letters)
    {
        ArgumentNullException.ThrowIfNull(letters);

        Height = height;
        LetterSpacing = letterSpacing;
        WordSpacing = wordSpacing;
        _letters = letters;
    }

    public int Height { get; }

    public int LetterSpacing { get; }

    public int WordSpacing { get; }

    public int MeasureWidth(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var width = 0;
        for (var i = 0; i < text.Length; i++)
        {
            width += text[i] == ' ' ? WordSpacing : _letters[text[i]][0].Length;
            if (NeedsLetterSpacing(text, i))
            {
                width += LetterSpacing;
            }
        }

        return width;
    }

    public void Draw(FrameBuffer buffer, int x, int y, string text, IReadOnlyList<Rgb> letterColors)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(letterColors);

        int cursor = x;
        var letterIndex = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                cursor += WordSpacing;
                continue;
            }

            string[] pattern = _letters[text[i]];
            DrawLetter(buffer, cursor, y, pattern, letterColors[letterIndex % letterColors.Count]);
            letterIndex++;
            cursor += pattern[0].Length;
            if (NeedsLetterSpacing(text, i))
            {
                cursor += LetterSpacing;
            }
        }
    }

    private static bool NeedsLetterSpacing(string text, int index) =>
        index + 1 < text.Length && text[index] != ' ' && text[index + 1] != ' ';

    private static void DrawLetter(FrameBuffer buffer, int x, int y, string[] pattern, Rgb color)
    {
        for (var row = 0; row < pattern.Length; row++)
        {
            for (var column = 0; column < pattern[row].Length; column++)
            {
                if (pattern[row][column] == PatternBlock)
                {
                    buffer.Put(x + column, y + row, Glyphs.FullBlock, color);
                }
            }
        }
    }
}
