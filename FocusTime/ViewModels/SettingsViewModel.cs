using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FocusTime.Models;
using Avalonia.Controls;

namespace FocusTime.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private TimerSettings _settings = new();
    
    private Window? _parentWindow;

    public SettingsViewModel()
    {
        LoadSettings();
    }

    public SettingsViewModel(TimerSettings settings)
    {
        Settings = settings;
    }
    
    public void SetParentWindow(Window? window)
    {
        _parentWindow = window;
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        Settings.FocusMinutes = 25;
        Settings.ShortBreakMinutes = 5;
        Settings.LongBreakMinutes = 15;
        Settings.SessionsUntilLongBreak = 4;
        Settings.AutoStartBreaks = false;
        Settings.AutoStartFocus = false;
        Settings.SoundEnabled = true;
        Settings.SoundVolume = 0.5;
        Settings.NotificationsEnabled = true;
    }

    [RelayCommand]
    private void SaveAndClose()
    {
        SaveSettings();
        _parentWindow?.Close();
    }

    private void LoadSettings()
    {
        // 这里可以从文件或注册表加载设置
        // 暂时使用默认值
    }

    private void SaveSettings()
    {
        // 这里可以保存设置到文件或注册表
    }
}