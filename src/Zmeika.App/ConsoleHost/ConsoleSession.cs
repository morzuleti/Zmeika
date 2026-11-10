using System.Runtime.CompilerServices;
using System.Text;

namespace Zmeika.App.ConsoleHost;

internal sealed class ConsoleSession : IDisposable
{
    private const string FontName = "Consolas";
    private const short FontHeight = 18;
    private const int TrueTypeModernFontFamily = 54;
    private const int NormalFontWeight = 400;

    private readonly int _width;
    private readonly int _height;
    private readonly nint _input;
    private readonly nint _output;
    private readonly uint? _originalInputMode;
    private readonly uint? _originalOutputMode;
    private bool _disposed;

    private ConsoleSession(int width, int height)
    {
        _width = width;
        _height = height;
        _input = NativeMethods.GetStdHandle(NativeMethods.StandardInputHandle);
        _output = NativeMethods.GetStdHandle(NativeMethods.StandardOutputHandle);
        _originalInputMode = ReadMode(_input);
        _originalOutputMode = ReadMode(_output);
    }

    public static ConsoleSession Open(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        ConsoleSession session = new(width, height);
        session.Configure();
        return session;
    }

    public bool HasRequiredSize => Console.WindowWidth >= _width && Console.WindowHeight >= _height;

    public bool EnsureSize()
    {
        if (Console.WindowWidth == _width && Console.WindowHeight == _height)
        {
            return false;
        }

        Resize();
        Console.CursorVisible = false;
        return true;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Console.Write("\e[0m\e[2J\e[H");
        Console.CursorVisible = true;
        RestoreMode(_input, _originalInputMode);
        RestoreMode(_output, _originalOutputMode);
    }

    private static uint? ReadMode(nint handle) =>
        NativeMethods.GetConsoleMode(handle, out uint mode) ? mode : null;

    private static void RestoreMode(nint handle, uint? mode)
    {
        if (mode is { } value)
        {
            NativeMethods.SetConsoleMode(handle, value);
        }
    }

    private void Configure()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.TreatControlCAsInput = true;
        Console.Title = "Zmeika";
        EnableVirtualTerminal();
        DisableQuickEdit();
        SetFont();
        Resize();
        Console.CursorVisible = false;
        Console.Write("\e[0m\e[2J");
    }

    private void EnableVirtualTerminal()
    {
        if (_originalOutputMode is { } mode)
        {
            NativeMethods.SetConsoleMode(_output, mode | NativeMethods.EnableVirtualTerminalProcessing);
        }
    }

    private void DisableQuickEdit()
    {
        if (_originalInputMode is { } mode)
        {
            uint withoutQuickEdit = (mode | NativeMethods.EnableExtendedFlags) & ~NativeMethods.EnableQuickEditMode;
            NativeMethods.SetConsoleMode(_input, withoutQuickEdit);
        }
    }

    private void SetFont()
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
        NativeMethods.SetCurrentConsoleFontEx(_output, false, ref fontInfo);
    }

    private void Resize()
    {
        try
        {
            Console.SetWindowSize(Math.Min(Console.WindowWidth, _width), Math.Min(Console.WindowHeight, _height));
            Console.SetBufferSize(_width, _height);
            Console.SetWindowSize(_width, _height);
        }
        catch (Exception exception) when (exception is IOException or ArgumentOutOfRangeException)
        {
            Console.Title = $"Zmeika: window could not be resized to {_width}x{_height}";
        }
    }
}
