using System.Diagnostics;
using System.Globalization;
using NAudio.Wave;
using Zmeika.App.Audio;

namespace Zmeika.App.ConsoleHost;

internal static class DebugHooks
{
    private const string CrashTestFlag = "--crash-test";
    private const string SoundTestFlag = "--sound-test";
    private const int SoundTestSteps = 60;
    private const int StepsPerSecond = 10;

    [Conditional("DEBUG")]
    public static void ThrowIfCrashTestRequested(IReadOnlyCollection<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (arguments.Contains(CrashTestFlag))
        {
            throw new InvalidOperationException("Crash test requested with " + CrashTestFlag + ".");
        }
    }

    [Conditional("DEBUG")]
    public static void RenderSoundTestIfRequested(IReadOnlyList<string> arguments, AudioMap map)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ArgumentNullException.ThrowIfNull(map);

        int index = arguments.ToList().IndexOf(SoundTestFlag);
        if (index < 0 || index + 1 >= arguments.Count)
        {
            return;
        }

        string path = arguments[index + 1];
        AudioMixer mixer = new(map);
        float[] step = new float[mixer.Format.SampleRate / StepsPerSecond * mixer.Format.Channels];
        using (WaveFileWriter writer = new(path, mixer.Format))
        {
            mixer.PlayLevelMusic(1);
            for (var i = 0; i < SoundTestSteps; i++)
            {
                PlaySoundTestCue(mixer, i);
                mixer.Output.Read(step);
                writer.WriteSamples(step, 0, step.Length);
            }
        }

        Console.Out.WriteLine(string.Create(CultureInfo.InvariantCulture, $"Sound test: {mixer.LoadedSoundCount} sounds loaded, written to {path}"));
        Environment.Exit(0);
    }

    private static void PlaySoundTestCue(AudioMixer mixer, int step)
    {
        switch (step)
        {
            case 10:
                mixer.PlayEffect(SoundEffect.Apple);
                break;
            case 20:
                mixer.PlayEffect(SoundEffect.Heart);
                break;
            case 25:
                mixer.PauseMusic();
                break;
            case 30:
                mixer.ResumeMusic();
                break;
            case 35:
                mixer.PlayEffect(SoundEffect.HitWall);
                break;
            case 40:
                mixer.PlayJingle(SoundEffect.LevelComplete);
                break;
            default:
                break;
        }
    }
}
