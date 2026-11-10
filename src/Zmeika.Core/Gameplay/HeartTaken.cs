using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record HeartTaken(Position Cell) : LevelEvent;
