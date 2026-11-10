namespace Zmeika.App.Screens;

internal interface INavigator
{
    void Show(IScreen screen);

    void Exit();
}
