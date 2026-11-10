using Zmeika.App.Rendering;

namespace Zmeika.App.Screens;

internal interface IScreen
{
    void OnEnter()
    {
    }

    void OnLeave()
    {
    }

    void HandleKey(ConsoleKeyInfo key);

    void Update(TimeSpan elapsed);

    void Paint(FrameBuffer buffer);
}
