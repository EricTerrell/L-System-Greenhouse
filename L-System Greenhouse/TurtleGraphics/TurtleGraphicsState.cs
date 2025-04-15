using System;
using Avalonia;

namespace L_System_Greenhouse.TurtleGraphics;

public class TurtleGraphicsState(
    float lineLength = 50,
    Point position = new(),
    float heading = 0,
    float turnAngle = 30,
    float turnAngleIncrement = 10,
    float lineWidth = 5,
    float lineWidthIncrement = 2,
    float lineLengthScaleFactor = 0.75f) : ICloneable
{
    protected bool Equals(TurtleGraphicsState other)
    {
        return LineLength.Equals(other.LineLength) && Position.Equals(other.Position) && 
               Heading.Equals(other.Heading) && TurnAngle.Equals(other.TurnAngle) && 
               TurnAngleIncrement.Equals(other.TurnAngleIncrement) && LineWidth.Equals(other.LineWidth) && 
               LineWidthIncrement.Equals(other.LineWidthIncrement) && 
               LineLengthScaleFactor.Equals(other.LineLengthScaleFactor);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TurtleGraphicsState)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LineLength, Position, Heading, TurnAngle, TurnAngleIncrement, LineWidth, 
            LineWidthIncrement, LineLengthScaleFactor);
    }

    public static bool operator ==(TurtleGraphicsState? left, TurtleGraphicsState? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TurtleGraphicsState? left, TurtleGraphicsState? right)
    {
        return !Equals(left, right);
    }

    public float LineLength { get; set; } = lineLength;

    public Point Position
    {
        get; set;
    } = position;
    
    public float Heading { get; set; } = heading;
    public float TurnAngle { get; set; } = turnAngle;
    public float TurnAngleIncrement { get; set; } = turnAngleIncrement;
    public float LineWidth { get; set; } = lineWidth;
    public float LineWidthIncrement { get; set; } = lineWidthIncrement;
    public float LineLengthScaleFactor { get; set; } = lineLengthScaleFactor;
    
    public object Clone()
    {
        return MemberwiseClone();
    }
    
    public static TurtleGraphicsState FromUI(ViewModels.TurtleGraphicsState uiTurtleGraphicsState)
    {
        var turtleGraphicsState = new TurtleGraphicsState
        {
            LineLength            = uiTurtleGraphicsState.LineLength,
            LineWidth             = uiTurtleGraphicsState.LineWidth,
            Heading               = uiTurtleGraphicsState.Heading,
            TurnAngle             = uiTurtleGraphicsState.TurnAngle,
            TurnAngleIncrement    = uiTurtleGraphicsState.TurnAngleIncrement,
            LineWidthIncrement    = uiTurtleGraphicsState.LineWidthIncrement,
            LineLengthScaleFactor = uiTurtleGraphicsState.LineLengthScaleFactor
        };

        return turtleGraphicsState;
    }

    public void ToUI(ViewModels.TurtleGraphicsState uiTurtleGraphicsState)
    {
        uiTurtleGraphicsState.LineLength            = LineLength;
        uiTurtleGraphicsState.LineWidth             = LineWidth;
        uiTurtleGraphicsState.Heading               = Heading;
        uiTurtleGraphicsState.TurnAngle             = TurnAngle;
        uiTurtleGraphicsState.TurnAngleIncrement    = TurnAngleIncrement;
        uiTurtleGraphicsState.LineWidthIncrement    = LineWidthIncrement;
        uiTurtleGraphicsState.LineLengthScaleFactor = LineLengthScaleFactor;
    }
}