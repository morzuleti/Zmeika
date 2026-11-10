using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record AppleEaten(Position Cell, int Score, int ApplesEaten) : GameEvent;
