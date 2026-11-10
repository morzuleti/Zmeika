using Zmeika.App.Input;

namespace Zmeika.App.Screens;

internal sealed class MenuSelection
{
    private readonly int _count;

    public MenuSelection(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        _count = count;
    }

    public int Index { get; private set; }

    public bool TryMove(ConsoleKeyInfo key)
    {
        if (GameKeys.IsMenuUp(key))
        {
            Index = (Index + _count - 1) % _count;
            return true;
        }

        if (GameKeys.IsMenuDown(key))
        {
            Index = (Index + 1) % _count;
            return true;
        }

        return false;
    }
}
