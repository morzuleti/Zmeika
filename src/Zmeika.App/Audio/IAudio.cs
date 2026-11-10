namespace Zmeika.App.Audio;

internal interface IAudio : IDisposable
{
    void PlayLevelMusic(int level);

    void PlayJingle(SoundEffect jingle);

    void PauseMusic();

    void ResumeMusic();

    void StopMusic();

    void PlayEffect(SoundEffect effect);
}
