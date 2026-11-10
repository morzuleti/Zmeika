namespace Zmeika.App.Rendering;

internal static class ScreenLayout
{
    public const int FieldColumns = 18;
    public const int FieldRows = 18;
    public const int CellWidth = 2;
    public const int FieldX = 0;
    public const int FieldY = 0;
    public const int FieldWidth = (FieldColumns * CellWidth) + 2;
    public const int FieldHeight = FieldRows + 2;

    public const int HudX = FieldX + FieldWidth + 1;
    public const int HudY = 0;
    public const int HudWidth = 25;
    public const int HudHeight = FieldHeight;
    public const int HudTextX = HudX + 3;

    public const int Width = HudX + HudWidth;
    public const int Height = FieldHeight;

    public static int CenterX(int areaX, int areaWidth, int contentWidth) => areaX + ((areaWidth - contentWidth) / 2);
}
