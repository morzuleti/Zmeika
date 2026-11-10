using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record HeartEaten(Position Cell, int Lives) : GameEvent;
