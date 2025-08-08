using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace FocusTime.Models;

public partial class TimerSettings : ObservableObject
{
    [ObservableProperty]
    private int _focusMinutes = 25;
    
    [ObservableProperty]
    private int _shortBreakMinutes = 5;
    
    [ObservableProperty]
    private int _longBreakMinutes = 15;
    
    [ObservableProperty]
    private int _sessionsUntilLongBreak = 4;
    
    [ObservableProperty]
    private bool _autoStartBreaks = false;
    
    [ObservableProperty]
    private bool _autoStartFocus = false;
    
    [ObservableProperty]
    private bool _soundEnabled = true;
    
    [ObservableProperty]
    private double _soundVolume = 0.5;
    
    [ObservableProperty]
    private bool _notificationsEnabled = true;
}

public enum SessionType
{
    Focus,
    ShortBreak,
    LongBreak
}

public partial class SessionStats : ObservableObject
{
    [ObservableProperty]
    private int _completedSessions = 0;
    
    [ObservableProperty]
    private int _todayCompletedSessions = 0;
    
    [ObservableProperty]
    private TimeSpan _totalFocusTime = TimeSpan.Zero;
    
    [ObservableProperty]
    private TimeSpan _todayFocusTime = TimeSpan.Zero;
    
    [ObservableProperty]
    private DateTime _lastSessionDate = DateTime.Now;
    
    [ObservableProperty]
    private int _currentStreak = 0;
}