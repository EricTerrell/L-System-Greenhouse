using Avalonia;

namespace L_System_Greenhouse.Tests;

using TurtleGraphics;

[TestClass]
public sealed class TurtleGraphicsTests
{
    [TestMethod]
    public void TestTurtleGraphics()
    {
        TurtleGraphicsState[] states = [ new(), new(), new() ];

        states[1].Position = new Point(0, 50);
        states[2].Position = new Point(0, 100);
        
        List<TurtleCommand> expectedResult =
        [
            new (Action.Start,                new Point(0, 0),  states[0]),
            new (Action.MoveForwardVisibly,   new Point(0, 0),  states[1]),
            new (Action.MoveForwardInvisibly, new Point(0, 50), states[2])
        ];

        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            var actualResult =
                ConvertToTurtleGraphics.Convert("Ff".ToList(), states[0], cancellationTokenSource.Token);
            
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }
    }
}