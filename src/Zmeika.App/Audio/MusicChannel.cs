using NAudio.Wave;

namespace Zmeika.App.Audio;

internal sealed class MusicChannel(float volume, WaveFormat format) : ISampleProvider
{
    private readonly Lock _gate = new();
    private CachedSound? _sound;
    private int _position;
    private bool _loop;
    private bool _isPaused;

    public WaveFormat WaveFormat { get; } = format;

    public void Play(CachedSound? sound, bool loop)
    {
        lock (_gate)
        {
            _sound = sound;
            _position = 0;
            _loop = loop;
            _isPaused = false;
        }
    }

    public void Pause()
    {
        lock (_gate)
        {
            _isPaused = true;
        }
    }

    public void Resume()
    {
        lock (_gate)
        {
            _isPaused = false;
        }
    }

    public void Stop() => Play(null, loop: false);

    public int Read(Span<float> buffer)
    {
        lock (_gate)
        {
            int written = _isPaused ? 0 : Fill(buffer);
            buffer[written..].Clear();
            return buffer.Length;
        }
    }

    private int Fill(Span<float> buffer)
    {
        var written = 0;
        while (written < buffer.Length && _sound is { } sound)
        {
            int available = sound.Samples.Length - _position;
            if (available == 0)
            {
                if (_loop)
                {
                    _position = 0;
                    continue;
                }

                _sound = null;
                break;
            }

            int count = Math.Min(available, buffer.Length - written);
            for (var i = 0; i < count; i++)
            {
                buffer[written + i] = sound.Samples[_position + i] * volume;
            }

            _position += count;
            written += count;
        }

        return written;
    }
}
