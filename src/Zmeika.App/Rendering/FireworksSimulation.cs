namespace Zmeika.App.Rendering;

internal sealed class FireworksSimulation
{
    public static readonly TimeSpan ParticleLifetime = TimeSpan.FromSeconds(1.4);

    private const double RocketRowsPerSecond = 18;
    private const double GravityRowsPerSecondSquared = 8;
    private const int ParticlesPerBurst = 16;
    private const double MinParticleSpeed = 6;
    private const double MaxParticleSpeed = 10;
    private const double ColumnsPerRow = ScreenLayout.CellWidth;

    private readonly FireworksStyle _style;
    private readonly IReadOnlyList<Rgb> _burstColors;
    private readonly Random _random;
    private readonly int _width;
    private readonly int _height;
    private readonly List<FireworksRocket> _rockets = [];
    private readonly List<FireworksParticle> _particles = [];
    private TimeSpan _sinceLaunch;

    public FireworksSimulation(FireworksStyle style, IReadOnlyList<Rgb> burstColors, Random random, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(style);
        ArgumentNullException.ThrowIfNull(burstColors);
        ArgumentNullException.ThrowIfNull(random);
        ArgumentOutOfRangeException.ThrowIfZero(burstColors.Count);

        _style = style;
        _burstColors = burstColors;
        _random = random;
        _width = width;
        _height = height;
        _sinceLaunch = style.LaunchInterval;
    }

    public IReadOnlyList<FireworksRocket> Rockets => _rockets;

    public IReadOnlyList<FireworksParticle> Particles => _particles;

    public void Update(TimeSpan elapsed)
    {
        _sinceLaunch += elapsed;
        while (_sinceLaunch >= _style.LaunchInterval)
        {
            _sinceLaunch -= _style.LaunchInterval;
            for (var i = 0; i < _style.RocketsPerLaunch; i++)
            {
                Launch();
            }
        }

        double seconds = elapsed.TotalSeconds;
        MoveRockets(seconds);
        MoveParticles(elapsed, seconds);
    }

    private void Launch()
    {
        int column = _random.Next(2, _width - 2);
        double burstRow = _random.Next(2, Math.Max(3, _height / 2));
        _rockets.Add(new FireworksRocket(column, _height - 1, burstRow, RandomColor()));
    }

    private void MoveRockets(double seconds)
    {
        for (int i = _rockets.Count - 1; i >= 0; i--)
        {
            FireworksRocket rocket = _rockets[i];
            rocket.Row -= RocketRowsPerSecond * seconds;
            if (rocket.Row <= rocket.BurstRow)
            {
                _rockets.RemoveAt(i);
                Burst(rocket);
            }
        }
    }

    private void Burst(FireworksRocket rocket)
    {
        for (var i = 0; i < ParticlesPerBurst; i++)
        {
            double angle = 2 * Math.PI * i / ParticlesPerBurst;
            double speed = MinParticleSpeed + (_random.NextDouble() * (MaxParticleSpeed - MinParticleSpeed));
            Rgb color = _style.MixedParticleColors ? RandomColor() : rocket.BurstColor;
            _particles.Add(new FireworksParticle(
                rocket.Column,
                rocket.BurstRow,
                Math.Cos(angle) * speed * ColumnsPerRow,
                Math.Sin(angle) * speed,
                color));
        }
    }

    private void MoveParticles(TimeSpan elapsed, double seconds)
    {
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            FireworksParticle particle = _particles[i];
            particle.Age += elapsed;
            particle.Column += particle.ColumnsPerSecond * seconds;
            particle.Row += particle.RowsPerSecond * seconds;
            particle.RowsPerSecond += GravityRowsPerSecondSquared * seconds;

            bool isOffScreen = particle.Column < 0 || particle.Column >= _width || particle.Row >= _height;
            if (particle.Age >= ParticleLifetime || isOffScreen)
            {
                _particles.RemoveAt(i);
            }
        }
    }

    private Rgb RandomColor() => _burstColors[_random.Next(_burstColors.Count)];
}
