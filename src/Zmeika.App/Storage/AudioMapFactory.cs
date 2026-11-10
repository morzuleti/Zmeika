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
        SoundsDto sounds = document.Audio.Sounds;

        return new AudioMap(
            [.. document.Levels.Select(level => Path.Combine(folder, level.Music))],
            new Dictionary<SoundEffect, string>
            {
                [SoundEffect.Apple] = Path.Combine(folder, sounds.Apple),
                [SoundEffect.Heart] = Path.Combine(folder, sounds.Heart),
                [SoundEffect.BiteTail] = Path.Combine(folder, sounds.BiteTail),
                [SoundEffect.HitWall] = Path.Combine(folder, sounds.HitWall),
                [SoundEffect.LevelComplete] = Path.Combine(folder, sounds.LevelComplete),
                [SoundEffect.GameOver] = Path.Combine(folder, sounds.GameOver),
                [SoundEffect.Victory] = Path.Combine(folder, sounds.Victory),
            },
            Volume(document.Audio.MusicVolume, "audio.musicVolume"),
            Volume(document.Audio.EffectsVolume, "audio.effectsVolume"));
    }

    private static float Volume(double value, string path) =>
        double.IsFinite(value) && value is >= 0 and <= 1
            ? (float)value
            : throw new ConfigException($"{path} must be between 0 and 1.");
}
