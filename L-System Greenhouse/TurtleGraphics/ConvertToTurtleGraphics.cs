using System;
using System.Threading;
using Avalonia;
using L_System_Greenhouse.Utils;
using System.Collections.Generic;

namespace L_System_Greenhouse.TurtleGraphics;

public static class ConvertToTurtleGraphics
{
    public static List<TurtleCommand> Convert(List<char> input, TurtleGraphicsState turtleGraphicsState, 
        CancellationToken cancellationToken)
    {
        var previousPosition = new Point(turtleGraphicsState.Position.X, turtleGraphicsState.Position.Y);
        
        var result = new List<TurtleCommand>(
            [new TurtleCommand(Action.Start, previousPosition, turtleGraphicsState.Clone() as TurtleGraphicsState)]
            );

        var turtleStateStack = new Stack<TurtleGraphicsState>();

        var currentState = turtleGraphicsState.Clone() as TurtleGraphicsState;

        // Convert L-System output letters into turtle commands, using the command set here:
        // https://paulbourke.net/fractals/lsys/

        try
        {
            input.ForEach(letter =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                currentState = currentState.Clone() as TurtleGraphicsState;

                switch (letter)
                {
                    case 'F':
                    case 'f':
                    {
                        var previousPosition = new Point(currentState.Position.X, currentState.Position.Y);

                        var newPosition = new Point(
                            currentState.Position.X + (Math.Sin(AngleUtils.DegreesToRadians(currentState.Heading)) *
                                                       currentState.LineLength),
                            currentState.Position.Y + (Math.Cos(AngleUtils.DegreesToRadians(currentState.Heading)) *
                                                       currentState.LineLength));

                        currentState.Position = new Point(newPosition.X, newPosition.Y);

                        result.Add(new TurtleCommand(
                            letter == 'F' ? Action.MoveForwardVisibly : Action.MoveForwardInvisibly, previousPosition,
                            currentState));
                    }
                        break;

                    case '+':
                    {
                        currentState.Heading += currentState.TurnAngle;
                    }
                        break;

                    case '-':
                    {
                        currentState.Heading -= currentState.TurnAngle;
                    }
                        break;

                    case '|':
                    {
                        currentState.Heading += 180;
                    }
                        break;

                    case '[':
                    {
                        turtleStateStack.Push(currentState);
                    }
                        break;

                    case ']':
                    {
                        currentState = turtleStateStack.Pop();
                    }
                        break;

                    case '#':
                    {
                        currentState.LineWidth += turtleGraphicsState.LineWidthIncrement;
                    }
                        break;

                    case '!':
                    {
                        currentState.LineWidth -= turtleGraphicsState.LineWidthIncrement;
                    }
                        break;

                    case '>':
                    {
                        currentState.LineLength *= turtleGraphicsState.LineLengthScaleFactor;
                    }
                        break;

                    case '<':
                    {
                        currentState.LineLength /= turtleGraphicsState.LineLengthScaleFactor;
                    }
                        break;

                    case '(':
                    {
                        currentState.TurnAngle -= turtleGraphicsState.TurnAngleIncrement;
                    }
                        break;

                    case ')':
                    {
                        currentState.TurnAngle += turtleGraphicsState.TurnAngleIncrement;
                    }
                        break;
                }
            });
        }
        catch (OperationCanceledException)
        {
            result.Clear();
        }

        return result;
    }
}