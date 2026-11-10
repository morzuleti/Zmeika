using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record HeartExpired(Position Cell) : LevelEvent;
