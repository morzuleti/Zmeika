using System.Diagnostics;

namespace Zmeika.App.ConsoleHost;

internal static class ClassicConsoleLauncher
{
    private const string WindowsTerminalVariable = "WT_SESSION";
    private const string RelaunchedVariable = "ZMEIKA_RELAUNCHED";
    private const string ClassicConsoleHost = "conhost.exe";
    private const string ClassicConsoleWindowClass = "ConsoleWindowClass";

    public static bool TryRelaunchInClassicConsole(IReadOnlyList<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (WasRelaunched() || !IsHostedInPseudoConsole() || Console.IsInputRedirected || Console.IsOutputRedirected)
        {
            return false;
        }

        string? processPath = Environment.ProcessPath;
        if (processPath is null || IsDotnetHost(processPath))
        {
            return false;
        }

        ProcessStartInfo startInfo = new(ClassicConsoleHost)
        {
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add(processPath);
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        startInfo.Environment.Remove(WindowsTerminalVariable);
        startInfo.Environment[RelaunchedVariable] = "1";

        using Process? process = Process.Start(startInfo);
        return process is not null;
    }

    private static bool WasRelaunched() =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(RelaunchedVariable));

    private static bool IsHostedInPseudoConsole() =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(WindowsTerminalVariable))
        || !string.Equals(ConsoleWindowClassName(), ClassicConsoleWindowClass, StringComparison.Ordinal);

    private static string ConsoleWindowClassName()
    {
        const int MaxClassNameLength = 256;

        nint window = NativeMethods.GetConsoleWindow();
        if (window == 0)
        {
            return string.Empty;
        }

        char[] buffer = new char[MaxClassNameLength];
        int length = NativeMethods.GetClassName(window, buffer, buffer.Length);
        return new string(buffer, 0, Math.Max(length, 0));
    }

    private static bool IsDotnetHost(string processPath) =>
        string.Equals(Path.GetFileNameWithoutExtension(processPath), "dotnet", StringComparison.OrdinalIgnoreCase);
}
