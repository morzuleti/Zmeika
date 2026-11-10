using System.Runtime.InteropServices;

namespace Zmeika.App.ConsoleHost;

internal static partial class NativeMethods
{
    public const int StandardInputHandle = -10;
    public const int StandardOutputHandle = -11;
    public const uint EnableVirtualTerminalProcessing = 0x0004;
    public const uint EnableQuickEditMode = 0x0040;
    public const uint EnableExtendedFlags = 0x0080;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial nint GetStdHandle(int handleId);

    [LibraryImport("kernel32.dll")]
    public static partial nint GetConsoleWindow();

    [LibraryImport("user32.dll", EntryPoint = "GetClassNameW")]
    public static partial int GetClassName(nint window, [Out] char[] className, int maxCount);

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
