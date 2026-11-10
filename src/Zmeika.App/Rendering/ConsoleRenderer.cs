namespace Zmeika.App.Rendering;

internal sealed class ConsoleRenderer(int width, int height)
{
    private readonly FrameBuffer _previous = new(width, height);
    private bool _hasPrevious;

    public void Present(FrameBuffer frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        string output = AnsiFormatter.FormatChanges(frame, _hasPrevious ? _previous : null);
        if (output.Length > 0)
        {
            Console.Out.Write(output);
            Console.Out.Flush();
        }

        _previous.CopyFrom(frame);
        _hasPrevious = true;
    }

    public void Invalidate() => _hasPrevious = false;
}
