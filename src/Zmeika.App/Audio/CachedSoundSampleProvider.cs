using NAudio.Wave;

namespace Zmeika.App.Audio;

internal sealed class CachedSoundSampleProvider(CachedSound sound, float volume, WaveFormat format) : ISampleProvider
{
    private int _position;

    public WaveFormat WaveFormat { get; } = format;

    public int Read(Span<float> buffer)
    {
        int count = Math.Min(buffer.Length, sound.Samples.Length - _position);
        for (var i = 0; i < count; i++)
        {
            buffer[i] = sound.Samples[_position + i] * volume;
        }

        _position += count;
        return count;
    }
}
