using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia;
using Point = Avalonia.Point;

namespace L_System_Greenhouse.TurtleGraphics;

public class ConvertToLineData
{
    public record LineData(double PenThickness, List<Point> Points);

    private double _halfSide;

    /// <summary>
    /// Convert turtle point in turtle coordinates to control coordinates.
    /// </summary>
    /// <param name="turtlePoint">point in turtle coordinates</param>
    /// <returns>point in control coordinates</returns>
    private Point ReMap(Point turtlePoint)
    {
        return new Point(turtlePoint.X + _halfSide, _halfSide - turtlePoint.Y);
    }

    public List<LineData> Convert(List<TurtleCommand> turtleCommands, Rect bounds, CancellationToken cancellationToken,
        IProgress<string>? progress = null)
    {
        _halfSide = Math.Min(bounds.Width, bounds.Height) / 2;

        List<LineData> lines = [];
        
        if (turtleCommands.Count > 0)
        {
            try
            {
                lines = ComputeLineData(turtleCommands, cancellationToken, progress);

                if (lines.Count > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    progress?.Report("Adjusting lines to fit in the display surface");
                    
                    const int margin = 10;
                    var margin2X = margin * 2;

                    var firstPoint = lines[0].Points[0];

                    var minXY = new Point(firstPoint.X, firstPoint.Y);
                    var maxXY = new Point(firstPoint.X, firstPoint.Y);

                    lines.ForEach(line =>
                    {
                        line.Points.ForEach(point =>
                        {
                            minXY = new Point(Math.Min(minXY.X, point.X), Math.Min(minXY.Y, point.Y));
                            maxXY = new Point(Math.Max(maxXY.X, point.X), Math.Max(maxXY.Y, point.Y));
                        });
                    });

                    var xRange = maxXY.X - minXY.X;
                    var yRange = maxXY.Y - minXY.Y;

                    var xScale = (bounds.Size.Width - margin2X) / xRange;
                    var yScale = (bounds.Size.Height - margin2X) / yRange;

                    var minScale = Math.Min(xScale, yScale);

                    var xCenter = ((bounds.Width - margin2X) - (xRange * minScale)) / 2;
                    var yCenter = ((bounds.Height - margin2X) - (yRange * minScale)) / 2;

                    // Adjust the lines so that the drawing fits perfectly in the rectangle, and is centered.
                    lines = lines.Select(line =>
                    {
                        return line with
                        {
                            Points = line.Points.Select(point => new Point(
                                (point.X - minXY.X) * minScale + xCenter + margin,
                                (point.Y - minXY.Y) * minScale + yCenter + margin
                            )).ToList()
                        };
                    }).ToList();
                }
            }
            catch (OperationCanceledException)
            {
                // Empty
            }
        }

        return lines;
    }
    
    private List<LineData> ComputeLineData(List<TurtleCommand> turtleCommands, CancellationToken cancellationToken,
        IProgress<string>? progress = null)
    {
        var lineData = new List<LineData>();

        try
        {
            if (turtleCommands.Count > 0)
            {
                int turtleCommandsProcessed = 0;
                
                turtleCommands.ForEach(turtleCommand =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    turtleCommandsProcessed++;

                    if (turtleCommandsProcessed == 1 || turtleCommandsProcessed % 1_000_000 == 0)
                    {
                        progress?.Report($"Computed line data for {turtleCommandsProcessed:N0} of {turtleCommands.Count:N0} turtle commands");
                    }

                    if (turtleCommand.Action == Action.MoveForwardVisibly)
                    {
                        List<Point> points =
                            [ReMap(turtleCommand.PreviousPosition), ReMap(turtleCommand.State.Position)];

                        lineData.Add(new LineData(turtleCommand.State.LineWidth, points));
                    }
                });
            }
        }
        catch (OperationCanceledException)
        {
            lineData.Clear();
        }

        return lineData;
    }
}