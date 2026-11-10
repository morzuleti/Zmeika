using System.Diagnostics;

namespace Zmeika.App.ConsoleHost;

internal static class ClassicConsoleLauncher
{
    private const string WindowsTerminalVariable = "WT_SESSION";
    private const string ClassicConsoleHost = "conhost.exe";

    public static bool TryRelaunchInClassicConsole(IReadOnlyList<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (!IsHostedInWindowsTerminal() || Console.IsInputRedirected || Console.IsOutputRedirected)
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

        using Process? process = Process.Start(startInfo);
        return process is not null;
    }

    private static bool IsHostedInWindowsTerminal() =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(WindowsTerminalVariable));

    private static bool IsDotnetHost(string processPath) =>
        string.Equals(Path.GetFileNameWithoutExtension(processPath), "dotnet", StringComparison.OrdinalIgnoreCase);
}
