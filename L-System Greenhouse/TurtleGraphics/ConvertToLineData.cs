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

    public List<LineData> Convert(List<TurtleCommand> turtleCommands, Rect bounds, CancellationToken cancellationToken)
    {
        _halfSide = Math.Min(bounds.Width, bounds.Height) / 2;

        List<LineData> lines = [];
        
        if (turtleCommands.Count > 0)
        {
            try
            {
                lines = ComputeLineData(turtleCommands, cancellationToken);

                if (lines.Count > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

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
    
    private List<LineData> ComputeLineData(List<TurtleCommand> turtleCommands, CancellationToken cancellationToken)
    {
        var lineData = new List<LineData>();

        try
        {
            if (turtleCommands.Count > 0)
            {
                turtleCommands.ForEach(turtleCommand =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

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