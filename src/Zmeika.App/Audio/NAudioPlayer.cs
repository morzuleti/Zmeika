using NAudio;
using NAudio.Wave;

namespace Zmeika.App.Audio;

internal sealed class NAudioPlayer : IAudio
{
    private readonly AudioMixer _mixer;
    private readonly WaveOut _output = new();

    private NAudioPlayer(AudioMixer mixer)
    {
        _mixer = mixer;
    }

    public static IAudio Create(AudioMap map)
    {
        AudioMixer mixer = new(map);
        NAudioPlayer player = new(mixer);
        try
        {
            player._output.Init(mixer.Output);
            player._output.Play();
            return player;
        }
        catch (MmException)
        {
            player.Dispose();
            return new SilentAudio();
        }
    }

    public void PlayLevelMusic(int level) => _mixer.PlayLevelMusic(level);

    public void PlayJingle(SoundEffect jingle) => _mixer.PlayJingle(jingle);

    public void PauseMusic() => _mixer.PauseMusic();

    public void ResumeMusic() => _mixer.ResumeMusic();

    public void StopMusic() => _mixer.StopMusic();

    public void PlayEffect(SoundEffect effect) => _mixer.PlayEffect(effect);

    public void Dispose()
    {
        _output.Stop();
        _output.Dispose();
    }
}
