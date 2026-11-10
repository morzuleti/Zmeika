using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Zmeika.App.Audio;

internal sealed class AudioMixer
{
    private const int SampleRate = 44100;
    private const int Channels = 2;

    private readonly MixingSampleProvider _mixer;
    private readonly MusicChannel _music;
    private readonly IReadOnlyList<CachedSound?> _levelMusic;
    private readonly IReadOnlyDictionary<SoundEffect, CachedSound?> _sounds;
    private readonly float _effectsVolume;

    public AudioMixer(AudioMap map)
    {
        ArgumentNullException.ThrowIfNull(map);

        Format = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);
        _levelMusic = [.. map.LevelMusicFiles.Select(path => CachedSound.TryLoad(path, Format))];
        _sounds = map.SoundFiles.ToDictionary(entry => entry.Key, entry => CachedSound.TryLoad(entry.Value, Format));
        _effectsVolume = map.EffectsVolume;

        _music = new MusicChannel(map.MusicVolume, Format);
        _mixer = new MixingSampleProvider(Format) { ReadFully = true };
        _mixer.AddMixerInput(_music);
    }

    public WaveFormat Format { get; }

    public ISampleProvider Output => _mixer;

    public int LoadedSoundCount => _levelMusic.Count(sound => sound is not null) + _sounds.Values.Count(sound => sound is not null);

    public void PlayLevelMusic(int level)
    {
        int index = level - 1;
        _music.Play(index >= 0 && index < _levelMusic.Count ? _levelMusic[index] : null, loop: true);
    }

    public void PlayJingle(SoundEffect jingle) => _music.Play(_sounds.GetValueOrDefault(jingle), loop: false);

    public void PauseMusic() => _music.Pause();

    public void ResumeMusic() => _music.Resume();

    public void StopMusic() => _music.Stop();

    public void PlayEffect(SoundEffect effect)
    {
        if (_sounds.GetValueOrDefault(effect) is { } sound)
        {
            _mixer.AddMixerInput(new CachedSoundSampleProvider(sound, _effectsVolume, Format));
        }
    }
}
