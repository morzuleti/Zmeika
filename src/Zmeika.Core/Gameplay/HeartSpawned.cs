using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record HeartSpawned(Position Cell) : GameEvent;
