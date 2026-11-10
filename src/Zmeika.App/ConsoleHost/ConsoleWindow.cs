using System.Runtime.CompilerServices;
using System.Text;

namespace Zmeika.App.ConsoleHost;

internal static class ConsoleWindow
{
    private const string FontName = "Consolas";
    private const short FontHeight = 18;
    private const int TrueTypeModernFontFamily = 54;
    private const int NormalFontWeight = 400;

    public static void Configure(int width, int height)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.TreatControlCAsInput = true;
        EnableVirtualTerminal();
        SetFont();
        Resize(width, height);
        Console.CursorVisible = false;
        Console.Write("\e[0m\e[2J");
    }

    public static void Restore()
    {
        Console.Write("\e[0m\e[2J\e[H");
        Console.CursorVisible = true;
    }

    private static void EnableVirtualTerminal()
    {
        nint output = NativeMethods.GetStdHandle(NativeMethods.StandardOutputHandle);
        if (NativeMethods.GetConsoleMode(output, out uint mode))
        {
            NativeMethods.SetConsoleMode(output, mode | NativeMethods.EnableVirtualTerminalProcessing);
        }
    }

    private static void SetFont()
    {
        ConsoleFontInfoEx fontInfo = new()
        {
            Size = (uint)Unsafe.SizeOf<ConsoleFontInfoEx>(),
            FontWidth = 0,
            FontHeight = FontHeight,
            FontFamily = TrueTypeModernFontFamily,
            FontWeight = NormalFontWeight,
        };
        FontName.AsSpan().CopyTo(fontInfo.FaceName);

        nint output = NativeMethods.GetStdHandle(NativeMethods.StandardOutputHandle);
        NativeMethods.SetCurrentConsoleFontEx(output, false, ref fontInfo);
    }

    private static void Resize(int width, int height)
    {
        try
        {
            Console.SetWindowSize(Math.Min(Console.WindowWidth, width), Math.Min(Console.WindowHeight, height));
            Console.SetBufferSize(width, height);
            Console.SetWindowSize(width, height);
        }
        catch (Exception exception) when (exception is IOException or ArgumentOutOfRangeException)
        {
            Console.Title = $"Zmeika: window could not be resized to {width}x{height}";
        }
    }
}
