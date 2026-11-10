namespace Zmeika.Core.Gameplay;

public sealed class SystemRandomSource(int? seed = null) : IRandomSource
{
    private readonly Random _random = seed is { } value ? new Random(value) : new Random();

    public int NextIndex(int count) => _random.Next(count);

    public TimeSpan NextDuration(TimeSpan min, TimeSpan max) => min + ((max - min) * _random.NextDouble());
}
