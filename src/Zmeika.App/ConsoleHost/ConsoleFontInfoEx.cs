using System.Runtime.InteropServices;

namespace Zmeika.App.ConsoleHost;

[StructLayout(LayoutKind.Sequential)]
internal struct ConsoleFontInfoEx
{
    public uint Size;
    public uint FontIndex;
    public short FontWidth;
    public short FontHeight;
    public int FontFamily;
    public int FontWeight;
    public FaceNameBuffer FaceName;
}
