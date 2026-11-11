using Zmeika.App.Audio;
using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

internal static class AudioMapFactory
{
    public const string SoundsFolder = "sounds";

    public static AudioMap Create(GameConfigDto document, string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(baseDirectory);

        string folder = Path.Combine(baseDirectory, SoundsFolder);

        return new AudioMap(
            [.. document.Levels.Select(level => Path.Combine(folder, level.Music))],
            SoundFiles(document.Audio.Sounds, folder),
            Volume(document.Audio.MusicVolume, "audio.musicVolume"),
            Volume(document.Audio.EffectsVolume, "audio.effectsVolume"));
    }

    private static Dictionary<SoundEffect, string> SoundFiles(SoundsDto sounds, string folder)
    {
        Dictionary<SoundEffect, string?> names = new()
        {
            [SoundEffect.Intro] = sounds.Intro,
            [SoundEffect.Apple] = sounds.Apple,
            [SoundEffect.Heart] = sounds.Heart,
            [SoundEffect.BiteTail] = sounds.BiteTail,
            [SoundEffect.HitWall] = sounds.HitWall,
            [SoundEffect.LifeLost] = sounds.LifeLost,
            [SoundEffect.LevelComplete] = sounds.LevelComplete,
            [SoundEffect.GameOver] = sounds.GameOver,
            [SoundEffect.Victory] = sounds.Victory,
        };

        Dictionary<SoundEffect, string> files = [];
        foreach ((SoundEffect effect, string? name) in names)
        {
            if (name is not null)
            {
                files[effect] = Path.Combine(folder, name);
            }
        }

        return files;
    }

    private static float Volume(double value, string path) =>
        double.IsFinite(value) && value is >= 0 and <= 1
            ? (float)value
            : throw new ConfigException($"{path} must be between 0 and 1.");
}
