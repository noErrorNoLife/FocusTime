using CommunityToolkit.Mvvm.ComponentModel;

namespace FocusTime.ViewModels;

public partial class SessionDotViewModel : ObservableObject
{
    [ObservableProperty]
    private string _color;

    public SessionDotViewModel(string color)
    {
        _color = color;
    }
} 