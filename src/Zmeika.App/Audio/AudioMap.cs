namespace Zmeika.App.Audio;

internal sealed record AudioMap(
    IReadOnlyList<string> LevelMusicFiles,
    IReadOnlyDictionary<SoundEffect, string> SoundFiles,
    float MusicVolume,
    float EffectsVolume);
