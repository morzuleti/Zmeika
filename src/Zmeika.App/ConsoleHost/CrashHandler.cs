using System.Globalization;

namespace Zmeika.App.ConsoleHost;

internal static class CrashHandler
{
    public const string RelativeLogPath = "data/error.log";
    public const int CrashExitCode = 2;

    public static void Install(ConsoleSession console, string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(baseDirectory);

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
        {
            console.Dispose();
            bool logged = TryAppendLog(baseDirectory, eventArgs.ExceptionObject);
            StartupMessage.Show(logged
                ? $"Zmeika stopped because of an unexpected error. Details are in {RelativeLogPath}."
                : "Zmeika stopped because of an unexpected error.");
            Environment.Exit(CrashExitCode);
        };
    }

    private static bool TryAppendLog(string baseDirectory, object error)
    {
        try
        {
            string path = Path.Combine(baseDirectory, RelativeLogPath);
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string entry = string.Create(
                CultureInfo.InvariantCulture,
                $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}]{Environment.NewLine}{error}{Environment.NewLine}{Environment.NewLine}");
            File.AppendAllText(path, entry);
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}
