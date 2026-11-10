using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Zmeika.App.Storage;

internal static class JsonFiles
{
    private const string TemporarySuffix = ".tmp";

    public static T? Read<T>(string path, JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize(json, typeInfo);
    }

    public static void WriteAtomic<T>(string path, T value, JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(typeInfo);

        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string temporaryPath = path + TemporarySuffix;
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(value, typeInfo));
        try
        {
            File.Move(temporaryPath, path, overwrite: true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            File.Delete(temporaryPath);
            throw;
        }
    }
}
