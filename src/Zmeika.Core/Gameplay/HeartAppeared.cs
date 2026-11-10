using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record HeartAppeared(Position Cell) : LevelEvent;
