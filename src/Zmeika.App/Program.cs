using Zmeika.App.ConsoleHost;
using Zmeika.App.Preview;
using Zmeika.App.Storage;

const string PreviewFlag = "--preview";
const int ConfigErrorExitCode = 1;

if (ClassicConsoleLauncher.TryRelaunchInClassicConsole(args))
{
    return 0;
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

if (!args.Contains(PreviewFlag))
{
    StartupMessage.Show($"Zmeika is not playable yet. Start it with {PreviewFlag} to see the glyph preview.");
    return 0;
}

new GlyphPreview(config.Theme).Run();
return 0;
