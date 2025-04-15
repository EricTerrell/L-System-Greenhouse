using Avalonia.Controls;
using Avalonia.Interactivity;

namespace L_System_Greenhouse;

public partial class HelpWindow : Window
{
    public Documentation Documentation { get; } = new();

    public HelpWindow()
    {
        InitializeComponent();
        
        DataContext = this;
    }

    private void Close_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}