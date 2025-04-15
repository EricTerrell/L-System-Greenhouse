using CommunityToolkit.Mvvm.ComponentModel;

namespace L_System_Greenhouse.ViewModels;

public class Production : ObservableObject
{
    private string _letter;

    public string Letter
    {
        get => _letter;
        set
        {
            _letter = value;
            OnPropertyChanged();
        }
    }
    
    private string _replacementLetters;

    public string ReplacementLetters
    {
        get => _replacementLetters;
        set
        {
            _replacementLetters = value;
            OnPropertyChanged();
        }
    }
}