using Zmeika.App.Rendering;

namespace Zmeika.App.Preview;

internal interface IPreviewPage
{
    string Title { get; }

    bool IsFullScreen => false;

    string KeyHint => string.Empty;

    void Draw(FrameBuffer buffer);

    bool HandleKey(ConsoleKeyInfo key) => false;
}
