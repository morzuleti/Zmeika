using System.Runtime.InteropServices;

namespace Zmeika.App.ConsoleHost;

internal static partial class NativeMethods
{
    public const int StandardOutputHandle = -11;
    public const uint EnableVirtualTerminalProcessing = 0x0004;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial nint GetStdHandle(int handleId);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetConsoleMode(nint consoleHandle, out uint mode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleMode(nint consoleHandle, uint mode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetCurrentConsoleFontEx(
        nint consoleOutput,
        [MarshalAs(UnmanagedType.Bool)] bool maximumWindow,
        ref ConsoleFontInfoEx fontInfo);
}
