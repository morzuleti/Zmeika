namespace Zmeika.App.Input;

internal static class KeyReader
{
    public static IReadOnlyList<ConsoleKeyInfo> ReadAvailable()
    {
        List<ConsoleKeyInfo> keys = [];
        while (Console.KeyAvailable)
        {
            keys.Add(Console.ReadKey(intercept: true));
        }

        return keys;
    }
}
