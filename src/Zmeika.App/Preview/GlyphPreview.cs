using System.Globalization;
using Zmeika.App.ConsoleHost;
using Zmeika.App.Preview.Pages;
using Zmeika.App.Rendering;

namespace Zmeika.App.Preview;

internal sealed class GlyphPreview
{
    private readonly Theme _theme;
    private readonly IReadOnlyList<IPreviewPage> _pages;
    private readonly FrameBuffer _buffer = new(ScreenLayout.Width, ScreenLayout.Height);
    private int _pageIndex;

    public GlyphPreview(Theme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        _theme = theme;
        _pages =
        [
            new LevelScreenPage(theme),
            new PalettePage(theme),
            new GlyphLegendPage(theme),
            new LevelColorsPage(theme),
            new SnakePage(theme),
            new InterfacePage(theme),
            new LargeLettersPage(theme),
            new FireworksPage(theme),
        ];
    }

    public void Run()
    {
        if (Console.IsOutputRedirected || Console.IsInputRedirected)
        {
            WriteAllPagesAsPlainText();
            return;
        }

        using ConsoleSession console = ConsoleSession.Open(ScreenLayout.Width, ScreenLayout.Height);
        RunInteractive();
    }

    private void RunInteractive()
    {
        while (true)
        {
            IPreviewPage page = _pages[_pageIndex];
            Render(page);
            Console.Title = WindowTitle(page);
            Console.Out.Write(AnsiFormatter.Format(_buffer));
            Console.Out.Flush();

            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            if (IsExitKey(key))
            {
                return;
            }

            if (!page.HandleKey(key))
            {
                Navigate(key);
            }
        }
    }

    private void WriteAllPagesAsPlainText()
    {
        for (var i = 0; i < _pages.Count; i++)
        {
            _pageIndex = i;
            Render(_pages[i]);
            Console.Out.WriteLine(string.Create(CultureInfo.InvariantCulture, $"=== Page {i + 1}: {_pages[i].Title}"));
            Console.Out.Write(PlainTextFormatter.Format(_buffer));
        }
    }

    private void Render(IPreviewPage page)
    {
        _buffer.Clear();
        page.Draw(_buffer);
        if (!page.IsFullScreen)
        {
            DrawPageChrome(page);
        }
    }

    private void DrawPageChrome(IPreviewPage page)
    {
        string header = string.Create(
            CultureInfo.InvariantCulture,
            $"ZMEIKA GLYPH PREVIEW   page {_pageIndex + 1} / {_pages.Count}   {page.Title}");
        _buffer.Write(1, 0, header, _theme.Menu.Title);
        _buffer.Write(1, ScreenLayout.Height - 1, NavigationHint(page), _theme.Menu.Hint);
    }

    private string WindowTitle(IPreviewPage page) =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"Zmeika glyph preview - page {_pageIndex + 1} / {_pages.Count}: {page.Title} - {NavigationHint(page)}");

    private static string NavigationHint(IPreviewPage page)
    {
        string hint = $"{Glyphs.ArrowLeft} {Glyphs.ArrowRight} page   Esc exit";
        return page.KeyHint.Length == 0 ? hint : $"{hint}   {page.KeyHint}";
    }

    private void Navigate(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.RightArrow or ConsoleKey.PageDown or ConsoleKey.Enter:
                _pageIndex = (_pageIndex + 1) % _pages.Count;
                break;
            case ConsoleKey.LeftArrow or ConsoleKey.PageUp:
                _pageIndex = (_pageIndex + _pages.Count - 1) % _pages.Count;
                break;
            default:
                break;
        }
    }

    private static bool IsExitKey(ConsoleKeyInfo key) =>
        key.Key == ConsoleKey.Escape
        || (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control));
}
