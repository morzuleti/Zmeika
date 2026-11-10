using System.Diagnostics.CodeAnalysis;

namespace Zmeika.Core.Players;

public sealed class PlayerName : IEquatable<PlayerName>
{
    private PlayerName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static bool TryCreate(string? text, int maxLength, [NotNullWhen(true)] out PlayerName? name)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLength);

        string trimmed = text?.Trim() ?? string.Empty;
        bool isValid = trimmed.Length >= 1 && trimmed.Length <= maxLength && !trimmed.Any(char.IsControl);
        name = isValid ? new PlayerName(trimmed) : null;
        return isValid;
    }

    public bool Matches(string? other) =>
        other is not null && string.Equals(Value, other.Trim(), StringComparison.OrdinalIgnoreCase);

    public bool Equals(PlayerName? other) => other is not null && Matches(other.Value);

    public override bool Equals(object? obj) => obj is PlayerName other && Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;
}
