using Avalonia.Controls;
using FocusTime.ViewModels;
using SukiUI.Controls;

namespace FocusTime.Views;

public partial class SettingsWindow : SukiWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
    }
    
    public SettingsWindow(SettingsViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.SetParentWindow(this);
    }
}