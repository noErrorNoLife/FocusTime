using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FocusTime.Models;
using FocusTime.Views;
using Avalonia.Controls.ApplicationLifetimes;

namespace FocusTime.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private Timer? _timer;
    private DateTime _sessionStartTime;
    private TimeSpan _remainingTime = TimeSpan.FromMinutes(25);
    private int _completedInCycle = 0;

    // --- Icon Paths ---
    private const string PlayIcon = "M8,5.14V19.14L19,12.14L8,5.14Z";
    private const string PauseIcon = "M14,19H18V5H14M6,19H10V5H6";

    // --- Colors ---
    private const string AccentColor = "#ff9a9e";     // 柔粉色作为激活色
    private const string SecondaryColor = "#a8edea";  // 水蓝色作为次要色
    private const string InactiveColor = "#FFFFFF25"; // 半透明白色作为非激活色

    [ObservableProperty]
    private string _timeDisplay = "25:00";

    [ObservableProperty]
    private double _progress = 0;

    [ObservableProperty]
    private bool _isRunning = false;

    [ObservableProperty]
    private SessionType _currentSessionType = SessionType.Focus;

    [ObservableProperty]
    private string _sessionTypeText = "专注";

    [ObservableProperty]
    private TimerSettings _settings = new();

    [ObservableProperty]
    private SessionStats _stats = new();

    [ObservableProperty]
    private bool _isSettingsOpen = false;

    [ObservableProperty]
    private string _startPauseButtonIcon = PlayIcon;

    [ObservableProperty]
    private ObservableCollection<SessionDotViewModel> _sessionDots = new();

    [ObservableProperty]
    private bool _isTopmost = false;

    [ObservableProperty]
    private bool _isFloatingBarMode = false;

    private FloatingBarWindow? _floatingBarWindow;

    public MainWindowViewModel()
    {
        UpdateSessionDots();
    }

    [RelayCommand]
    private void StartPause()
    {
        if (IsRunning)
        {
            PauseTimer();
        }
        else
        {
            StartTimer();
        }
    }

    [RelayCommand]
    private void Reset()
    {
        StopTimer();
        ResetSession();
    }

    [RelayCommand]
    private void SkipSession()
    {
        StopTimer();
        CompleteCurrentSession();
    }

    [RelayCommand]
    private void ToggleTopmost()
    {
        IsTopmost = !IsTopmost;
    }

    [RelayCommand]
    private void ToggleFloatingBar()
    {
        IsFloatingBarMode = !IsFloatingBarMode;
        
        if (IsFloatingBarMode)
        {
            ShowFloatingBar();
        }
        else
        {
            HideFloatingBar();
        }
    }

    [RelayCommand]
    private void CloseFloatingBar()
    {
        IsFloatingBarMode = false;
        HideFloatingBar();
    }

    [RelayCommand]
    private async void OpenSettings()
    {
        var settingsViewModel = new SettingsViewModel(Settings);
        var settingsWindow = new SettingsWindow(settingsViewModel);
        IsSettingsOpen = true;
        
        // 通过服务定位器或依赖注入获取主窗口
        var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
            
        await settingsWindow.ShowDialog(mainWindow);
        IsSettingsOpen = false;
        UpdateTimerForNewSettings();
    }

    private void StartTimer()
    {
        IsRunning = true;
        SessionTypeText = CurrentSessionType == SessionType.Focus ? "专注" : "休息";
        StartPauseButtonIcon = PauseIcon;
        _sessionStartTime = DateTime.Now;
        
        _timer = new Timer(UpdateTimer, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void PauseTimer()
    {
        IsRunning = false;
        SessionTypeText = "已暂停";
        StartPauseButtonIcon = PlayIcon;
        _timer?.Dispose();
        _timer = null;
    }

    private void StopTimer()
    {
        IsRunning = false;
        _timer?.Dispose();
        _timer = null;
    }

    private void UpdateTimer(object? state)
    {
        var elapsed = DateTime.Now - _sessionStartTime;
        var sessionDuration = TimeSpan.FromMinutes(GetCurrentSessionMinutes());
        
        if (elapsed >= sessionDuration)
        {
            CompleteCurrentSession();
            return;
        }

        _remainingTime = sessionDuration - elapsed;
        TimeDisplay = $"{_remainingTime.Minutes:D2}:{_remainingTime.Seconds:D2}";
        Progress = (elapsed.TotalSeconds / sessionDuration.TotalSeconds) * 100;
    }

    private void CompleteCurrentSession()
    {
        StopTimer();
        
        if (CurrentSessionType == SessionType.Focus)
        {
            _completedInCycle++;
            Stats.CompletedSessions++;
            Stats.TodayCompletedSessions++;
            Stats.TotalFocusTime += TimeSpan.FromMinutes(Settings.FocusMinutes);
            Stats.TodayFocusTime += TimeSpan.FromMinutes(Settings.FocusMinutes);
            Stats.LastSessionDate = DateTime.Now;

            UpdateSessionDots();
            
            if (_completedInCycle >= Settings.SessionsUntilLongBreak)
            {
                _completedInCycle = 0;
                StartLongBreak();
            }
            else
            {
                StartShortBreak();
            }
        }
        else
        {
            StartWorkSession();
        }
        
        ShowSessionCompleteNotification();
    }

    private void StartWorkSession()
    {
        CurrentSessionType = SessionType.Focus;
        SessionTypeText = "专注";
        _remainingTime = TimeSpan.FromMinutes(Settings.FocusMinutes);
        ResetDisplay();
        
        if (Settings.AutoStartFocus && !IsRunning)
        {
            StartTimer();
        }
    }

    private void StartShortBreak()
    {
        CurrentSessionType = SessionType.ShortBreak;
        SessionTypeText = "短时休息";
        _remainingTime = TimeSpan.FromMinutes(Settings.ShortBreakMinutes);
        ResetDisplay();
        
        if (Settings.AutoStartBreaks && !IsRunning)
        {
            StartTimer();
        }
    }

    private void StartLongBreak()
    {
        CurrentSessionType = SessionType.LongBreak;
        SessionTypeText = "长时休息";
        _remainingTime = TimeSpan.FromMinutes(Settings.LongBreakMinutes);
        ResetDisplay();
        
        if (Settings.AutoStartBreaks && !IsRunning)
        {
            StartTimer();
        }
    }

    private void ResetSession()
    {
        StopTimer();
        
        switch (CurrentSessionType)
        {
            case SessionType.Focus:
                _remainingTime = TimeSpan.FromMinutes(Settings.FocusMinutes);
                SessionTypeText = "专注";
                break;
            case SessionType.ShortBreak:
                _remainingTime = TimeSpan.FromMinutes(Settings.ShortBreakMinutes);
                SessionTypeText = "短时休息";
                break;
            case SessionType.LongBreak:
                _remainingTime = TimeSpan.FromMinutes(Settings.LongBreakMinutes);
                SessionTypeText = "长时休息";
                break;
        }
        
        ResetDisplay();
    }

    private void ResetDisplay()
    {
        StartPauseButtonIcon = PlayIcon;
        TimeDisplay = $"{_remainingTime.Minutes:D2}:{_remainingTime.Seconds:D2}";
        Progress = 0;
        if (!IsRunning)
        {
            // When not running, SessionTypeText should reflect the ready state
            switch (CurrentSessionType)
            {
                case SessionType.Focus:
                    SessionTypeText = "专注";
                    break;
                case SessionType.ShortBreak:
                    SessionTypeText = "短时休息";
                    break;
                case SessionType.LongBreak:
                    SessionTypeText = "长时休息";
                    break;
            }
        }
    }

    private int GetCurrentSessionMinutes()
    {
        return CurrentSessionType switch
        {
            SessionType.Focus => Settings.FocusMinutes,
            SessionType.ShortBreak => Settings.ShortBreakMinutes,
            SessionType.LongBreak => Settings.LongBreakMinutes,
            _ => Settings.FocusMinutes
        };
    }

    private void UpdateTimerForNewSettings()
    {
        _remainingTime = TimeSpan.FromMinutes(GetCurrentSessionMinutes());
        ResetDisplay();
        UpdateSessionDots();
    }

    private void UpdateSessionDots()
    {
        SessionDots.Clear();
        for (int i = 0; i < Settings.SessionsUntilLongBreak; i++)
        {
            var color = i < _completedInCycle ? AccentColor : InactiveColor;
            SessionDots.Add(new SessionDotViewModel(color));
        }
    }

    private void ShowSessionCompleteNotification()
    {
        // This could be implemented with a notification library
    }

    private void ShowFloatingBar()
    {
        if (_floatingBarWindow == null)
        {
            _floatingBarWindow = new FloatingBarWindow();
            _floatingBarWindow.DataContext = this;
            _floatingBarWindow.Closed += (s, e) => 
            {
                _floatingBarWindow = null;
                IsFloatingBarMode = false;
            };
        }
        
        _floatingBarWindow.Show();
        
        // 可选：当悬浮条显示时最小化主窗口
        // 用户可以通过悬浮条继续操作
        var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        mainWindow?.Hide();
    }

    private void HideFloatingBar()
    {
        _floatingBarWindow?.Close();
        _floatingBarWindow = null;
        
        // 恢复主窗口显示
        var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
        mainWindow?.Show();
        mainWindow?.Activate();
    }
}