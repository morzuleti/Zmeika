using Zmeika.App.ConsoleHost;
using Zmeika.App.Hosting;
using Zmeika.App.Preview;
using Zmeika.App.Rendering;
using Zmeika.App.Screens;
using Zmeika.App.Storage;
using Zmeika.Core.Gameplay;

const string PreviewFlag = "--preview";
const int SuccessExitCode = 0;
const int ConfigErrorExitCode = 1;
const int NoConsoleExitCode = 3;
const int WindowTooSmallExitCode = 4;

if (ClassicConsoleLauncher.TryRelaunchInClassicConsole(args))
{
    return SuccessExitCode;
}

GameConfig config;
try
{
    config = GameConfigLoader.Load(AppContext.BaseDirectory);
}
catch (ConfigException exception)
{
    StartupMessage.Show(exception.Message);
    return ConfigErrorExitCode;
}

if (args.Contains(PreviewFlag))
{
    new GlyphPreview(config.Theme).Run();
    return SuccessExitCode;
}

if (Console.IsInputRedirected || Console.IsOutputRedirected)
{
    StartupMessage.Show("Zmeika needs an interactive console window.");
    return NoConsoleExitCode;
}

using ConsoleSession console = ConsoleSession.Open(ScreenLayout.Width, ScreenLayout.Height);
if (!console.HasRequiredSize)
{
    console.Dispose();
    StartupMessage.Show(
        $"The console window must be at least {ScreenLayout.Width} x {ScreenLayout.Height} characters. " +
        "Start Zmeika.exe directly so that it opens in the classic console.");
    return WindowTooSmallExitCode;
}

SettingsStore settings = SettingsStore.Load(AppContext.BaseDirectory, config.Theme);
PlayersStore players = PlayersStore.Load(AppContext.BaseDirectory);

GameHost host = new(console);
SystemRandomSource random = new();
GameFlow flow = new(config.Rules, random, players, TimeProvider.System);
GameContext context = new(config.Theme, config.Timings, host, config.Interface, settings.Current, flow);
await host.RunAsync(new NameInputScreen(context)).ConfigureAwait(false);
return SuccessExitCode;
