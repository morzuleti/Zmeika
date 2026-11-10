# Zmeika

A console Snake game in C# on .NET 10.

## Requirements

- .NET SDK 10.0.401 or newer 10.0.x (see `global.json`)

## Build, test, run

```bash
dotnet build
dotnet test
dotnet run --project src/Zmeika.App
```

## Structure

- `src/Zmeika.Core` — game logic (pure, deterministic, fully unit-tested)
- `src/Zmeika.App` — console host: input, game loop, rendering
- `tests/Zmeika.Core.Tests` — xUnit tests
