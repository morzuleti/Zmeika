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
    UseStringEnumConverter = true,
    WriteIndented = true)]
[JsonSerializable(typeof(GameConfigDto))]
[JsonSerializable(typeof(SettingsDto))]
[JsonSerializable(typeof(PlayersDto))]
internal sealed partial class StorageJsonContext : JsonSerializerContext;
