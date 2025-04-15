using Avalonia;

namespace L_System_Greenhouse.TurtleGraphics;

public enum Action
{
    Start,
    MoveForwardVisibly,
    MoveForwardInvisibly
}

public record TurtleCommand(Action Action, Point PreviousPosition, TurtleGraphicsState State);