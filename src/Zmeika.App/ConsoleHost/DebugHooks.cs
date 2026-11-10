using System.Diagnostics;

namespace Zmeika.App.ConsoleHost;

internal static class DebugHooks
{
    private const string CrashTestFlag = "--crash-test";

    [Conditional("DEBUG")]
    public static void ThrowIfCrashTestRequested(IReadOnlyCollection<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (arguments.Contains(CrashTestFlag))
        {
            throw new InvalidOperationException("Crash test requested with " + CrashTestFlag + ".");
        }
    }
}
