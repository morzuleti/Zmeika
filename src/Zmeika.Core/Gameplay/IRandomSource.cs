namespace Zmeika.Core.Gameplay;

public interface IRandomSource
{
    int NextIndex(int count);

    TimeSpan NextDuration(TimeSpan min, TimeSpan max);
}
