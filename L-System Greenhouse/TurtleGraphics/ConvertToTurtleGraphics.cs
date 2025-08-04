using System;
using System.Threading;
using Avalonia;
using L_System_Greenhouse.Utils;
using System.Collections.Generic;
using log4net;

namespace L_System_Greenhouse.TurtleGraphics;

public static class ConvertToTurtleGraphics
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    public static List<TurtleCommand> Convert(List<char> input, TurtleGraphicsState turtleGraphicsState, 
        CancellationToken cancellationToken, IProgress<string>? progress = null)
    {
        var startTime = DateTime.Now;
        
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
            var letterCount = 0;
            
            input.ForEach(letter =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                letterCount++;

                if (letterCount == 1 || letterCount % 1_000_000 == 0)
                {
                    progress?.Report(
                        $"Converting L-System output to turtle commands: Processing letter {letterCount:N0} of {input.Count:N0}");
                }

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

            if (turtleStateStack.Count > 0)
            {
                Log.Error($"turtleStateStack not empty. # items: {turtleStateStack.Count}");            
            }
        }
        catch (OperationCanceledException)
        {
            result.Clear();
        }

        Log.Info($"ConvertToTurtleGraphics.Convert: elapsed time: {DateTime.Now - startTime}");

        return result;
    }
}