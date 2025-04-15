using Avalonia.Controls;
using Avalonia.Interactivity;

namespace L_System_Greenhouse;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
    }

    private void Close_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}