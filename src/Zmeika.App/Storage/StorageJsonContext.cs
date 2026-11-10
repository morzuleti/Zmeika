using System.Text.Json;
using System.Text.Json.Serialization;
using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    WriteIndented = true)]
[JsonSerializable(typeof(GameConfigDto))]
internal sealed partial class StorageJsonContext : JsonSerializerContext;
