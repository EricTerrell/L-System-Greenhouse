using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace L_System_Greenhouse.ViewModels;

public class LSystem : ObservableObject
{
    private string _axiom = "F[+F]-F";

    public string Axiom
    {
        get => _axiom;
        
        set
        {
            _axiom = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Production> _productions = [];
    
    public ObservableCollection<Production> Productions
    {
        get => _productions;

        set
        {
            _productions = value;
            OnPropertyChanged();
        }
    }

    private int _iterations;

    public int Iterations
    {
        get => _iterations;

        set
        {
            _iterations = value;
            OnPropertyChanged();
        }
    }
}