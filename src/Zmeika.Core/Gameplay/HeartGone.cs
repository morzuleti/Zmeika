using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record HeartGone(Position Cell) : GameEvent;
