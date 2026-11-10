using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Zmeika.App.Audio;

internal sealed class CachedSound
{
    private const int StereoChannels = 2;

    private CachedSound(float[] samples)
    {
        Samples = samples;
    }

    public float[] Samples { get; }

    public static CachedSound? TryLoad(string path, WaveFormat format)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(format);

        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            using WaveFileReader reader = new(path);
            ISampleProvider source = reader.ToSampleProvider();
            if (source.WaveFormat.SampleRate != format.SampleRate)
            {
                source = new WdlResamplingSampleProvider(source, format.SampleRate);
            }

            if (source.WaveFormat.Channels == 1)
            {
                source = new MonoToStereoSampleProvider(source);
            }

            return source.WaveFormat.Channels == StereoChannels ? ReadAll(source, format) : null;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or FormatException
            or InvalidDataException or ArgumentException or NotSupportedException)
        {
            return null;
        }
    }

    private static CachedSound? ReadAll(ISampleProvider source, WaveFormat format)
    {
        List<float> samples = [];
        float[] chunk = new float[format.SampleRate * StereoChannels];
        int read;
        while ((read = source.Read(chunk)) > 0)
        {
            samples.AddRange(chunk.AsSpan(0, read));
        }

        return samples.Count == 0 ? null : new CachedSound([.. samples]);
    }
}
