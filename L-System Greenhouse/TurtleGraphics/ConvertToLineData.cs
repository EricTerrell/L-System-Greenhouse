using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia;
using Point = Avalonia.Point;
using log4net;

namespace L_System_Greenhouse.TurtleGraphics;

public class ConvertToLineData
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

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
        var startTime = DateTime.Now;
        
        _halfSide = Math.Min(bounds.Width, bounds.Height) / 2;

        List<LineData> lines = [];
        
        if (turtleCommands.Count > 0)
        {
            try
            {
                lines = ComputeLineData(
                    turtleCommands.Where(turtleCommand => turtleCommand.Action == Action.MoveForwardVisibly).ToList(), 
                    cancellationToken, progress);

                if (lines.Count > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    progress?.Report("Adjusting lines to fit in the display surface");
                    
                    const int margin = 10;
                    var margin2X = margin * 2;

                    var firstPoint = lines[0].Points[0];

                    var allPoints = lines.SelectMany(line => line.Points).ToList();
                    
                    var minXY = allPoints.Aggregate(firstPoint, (p1, p2) => 
                        new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y)));
                    
                    var maxXY = allPoints.Aggregate(firstPoint, (p1, p2) => 
                        new Point(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y)));
                    
                    var xRange = maxXY.X - minXY.X;
                    var yRange = maxXY.Y - minXY.Y;

                    var xScale = (bounds.Size.Width  - margin2X) / xRange;
                    var yScale = (bounds.Size.Height - margin2X) / yRange;

                    var minScale = Math.Min(xScale, yScale);

                    var xCenter = ((bounds.Width  - margin2X) - (xRange * minScale)) / 2;
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

        Log.Info($"ConvertToLineData.Convert: elapsed time: {DateTime.Now - startTime}");

        return lines;
    }
    
    private List<LineData> ComputeLineData(List<TurtleCommand> turtleCommands, CancellationToken cancellationToken,
        IProgress<string>? progress = null)
    {
        var startTime = DateTime.Now;
        
        var lineData = new List<LineData>();

        try
        {
            if (turtleCommands.Count > 0)
            {
                var turtleCommandsProcessed = 0;

                lineData = turtleCommands.Select(turtleCommand =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    turtleCommandsProcessed++;

                    if (turtleCommandsProcessed == 1 || turtleCommandsProcessed % 1_000_000 == 0)
                    {
                        progress?.Report($"Computed line data for {turtleCommandsProcessed:N0} of {turtleCommands.Count:N0} turtle commands");
                    }

                    return new LineData(turtleCommand.State.LineWidth, 
                        [ReMap(turtleCommand.PreviousPosition), ReMap(turtleCommand.State.Position)]);
                }).ToList();
            }
        }
        catch (OperationCanceledException)
        {
            lineData.Clear();
        }

        Log.Info($"\tConvertToLineData.ComputeLineData: elapsed time: {DateTime.Now - startTime}");

        return lineData;
    }
}