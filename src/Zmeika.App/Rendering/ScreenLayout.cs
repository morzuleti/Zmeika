namespace Zmeika.App.Rendering;

internal static class ScreenLayout
{
    public const int Width = 88;
    public const int Height = 32;

    public const int FieldColumns = 30;
    public const int FieldRows = 30;
    public const int CellWidth = 2;
    public const int FieldX = 0;
    public const int FieldY = 0;

    public const int HudX = 63;
    public const int HudY = 0;
    public const int HudWidth = 25;
    public const int HudHeight = 32;
    public const int HudTextX = HudX + 3;

    public static int CenterX(int areaX, int areaWidth, int contentWidth) => areaX + ((areaWidth - contentWidth) / 2);
}
