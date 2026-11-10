namespace Zmeika.App.Storage.Dto;

internal sealed record LeaderboardDto(
    string Title,
    string Header,
    string FirstPlace,
    string CurrentGame,
    string OtherGames);
