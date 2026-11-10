using Zmeika.Core.Model;

namespace Zmeika.Core.Gameplay;

public sealed record AppleTaken(Position Cell, int ApplesEaten) : LevelEvent;
