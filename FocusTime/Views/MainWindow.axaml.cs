using Avalonia.Controls;
using Avalonia.Input;
using SukiUI.Controls;

namespace FocusTime.Views;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
}