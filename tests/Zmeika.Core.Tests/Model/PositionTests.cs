using Zmeika.Core.Model;

namespace Zmeika.Core.Tests.Model;

public sealed class PositionTests
{
    [Theory]
    [InlineData(0, 0, 1, 0, 1, 0)]
    [InlineData(5, 5, -1, 0, 4, 5)]
    [InlineData(3, 3, 0, -1, 3, 2)]
    public void Offset_WithDelta_ReturnsShiftedPosition(int x, int y, int dx, int dy, int expectedX, int expectedY)
    {
        var position = new Position(x, y);

        var result = position.Offset(dx, dy);

        Assert.Equal(new Position(expectedX, expectedY), result);
    }
}
