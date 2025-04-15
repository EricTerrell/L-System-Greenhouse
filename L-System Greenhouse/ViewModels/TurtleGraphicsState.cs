using CommunityToolkit.Mvvm.ComponentModel;

namespace L_System_Greenhouse.ViewModels;

public class TurtleGraphicsState : ObservableObject
{
    private float
        _lineLength,
        _lineLengthScaleFactor,
        _heading,
        _turnAngle,
        _turnAngleIncrement,
        _lineWidth,
        _lineWidthIncrement;
    
    public float LineLength
    {
        get => _lineLength;
        
        set
        {
            _lineLength = value;
            OnPropertyChanged();
        }
    }
    
    public float LineLengthScaleFactor
    {
        get => _lineLengthScaleFactor;
        
        set
        {
            _lineLengthScaleFactor = value;
            OnPropertyChanged();
        }
    }
    
    public float Heading
    {
        get => _heading;
        
        set
        {
            _heading = value;
            OnPropertyChanged();
        }
    }
    
    public float TurnAngle
    {
        get => _turnAngle;
        
        set
        {
            _turnAngle = value;
            OnPropertyChanged();
        }
    }
    
    public float TurnAngleIncrement
    {
        get => _turnAngleIncrement;
        
        set
        {
            _turnAngleIncrement = value;
            OnPropertyChanged();
        }
    }
    
    public float LineWidth
    {
        get => _lineWidth;
        
        set
        {
            _lineWidth = value;
            OnPropertyChanged();
        }
    }
    
    public float LineWidthIncrement
    {
        get => _lineWidthIncrement;
        
        set
        {
            _lineWidthIncrement = value;
            OnPropertyChanged();
        }
    }
}