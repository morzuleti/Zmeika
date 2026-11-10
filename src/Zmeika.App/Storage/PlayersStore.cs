using System.Text.Json;
using Zmeika.App.Storage.Dto;
using Zmeika.Core.Players;

namespace Zmeika.App.Storage;

internal sealed class PlayersStore
{
    public const string RelativePath = "data/players.json";
    public const string BackupSuffix = ".bak";

    private readonly string _path;

    private PlayersStore(string path, PlayersBook book)
    {
        _path = path;
        Book = book;
    }

    public PlayersBook Book { get; }

    public static PlayersStore Load(string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);

        string path = Path.Combine(baseDirectory, RelativePath);
        if (!File.Exists(path))
        {
            return new PlayersStore(path, PlayersBook.Empty);
        }

        try
        {
            PlayersDto dto = JsonFiles.Read(path, StorageJsonContext.Default.PlayersDto)
                ?? throw new JsonException("The file contains no data.");
            return new PlayersStore(path, FromDto(dto));
        }
        catch (Exception exception) when (exception is JsonException or ArgumentException)
        {
            return StartFreshAfterBackup(path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return new PlayersStore(path, PlayersBook.Empty);
        }
    }

    public void Save() => JsonFiles.WriteAtomic(_path, ToDto(Book), StorageJsonContext.Default.PlayersDto);

    private static PlayersStore StartFreshAfterBackup(string path)
    {
        PlayersStore store = new(path, PlayersBook.Empty);
        try
        {
            File.Move(path, path + BackupSuffix, overwrite: true);
            store.Save();
            return store;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return store;
        }
    }

    private static PlayersBook FromDto(PlayersDto dto) => new(
        dto.Games.Select(game => new GameRecord(game.Id, game.Player, game.Score, game.StartedAt, game.Status)),
        dto.Progress.Select(entry => new PlayerProgress(entry.Player, entry.GameId, entry.Level, entry.Lives, entry.Score)));

    private static PlayersDto ToDto(PlayersBook book) => new(
        [.. book.Games.Select(game => new GameRecordDto(game.Id, game.Player, game.Score, game.StartedAt, game.Status))],
        [.. book.Progress.Select(entry => new ProgressDto(entry.Player, entry.GameId, entry.Level, entry.Lives, entry.Score))]);
}
