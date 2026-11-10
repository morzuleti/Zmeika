namespace Zmeika.App.ConsoleHost;

internal static class StartupMessage
{
    public static void Show(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Console.Out.WriteLine(message);
        if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
        {
            Console.Out.WriteLine("Press any key to exit.");
            Console.ReadKey(intercept: true);
        }
    }
}
