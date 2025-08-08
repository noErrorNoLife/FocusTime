using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using SukiUI.Controls;
using System;

namespace FocusTime.Views;

public partial class FloatingBarWindow : SukiWindow
{
    private bool _isDragging = false;
    private Point _lastPosition;

    public FloatingBarWindow()
    {
        InitializeComponent();
        
        // 设置初始位置
        SetInitialPosition();
        
        // 为主卡片添加拖动支持
        this.Loaded += (s, e) => {
            var mainCard = this.FindControl<SukiUI.Controls.GlassCard>("MainCard");
            if (mainCard != null)
            {
                mainCard.PointerPressed += OnCardPointerPressed;
                mainCard.PointerMoved += OnCardPointerMoved;
                mainCard.PointerReleased += OnCardPointerReleased;
            }
        };
        
    }

    private void SetInitialPosition()
    {
        // 获取屏幕尺寸
        var screen = Screens.Primary;
        if (screen != null)
        {
            var workingArea = screen.WorkingArea;
            
            // 初始位置：屏幕右上角
            Position = new PixelPoint(
                workingArea.X + workingArea.Width - (int)Width - 20,
                workingArea.Y + 50
            );
        }
    }

    private void OnCardPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // 只在点击的不是按钮时才开始拖动
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && 
            e.Source is not Button)
        {
            _isDragging = true;
            _lastPosition = e.GetPosition(this);
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }
    
    private void OnCardPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            var currentPosition = e.GetPosition(this);
            var deltaX = currentPosition.X - _lastPosition.X;
            var deltaY = currentPosition.Y - _lastPosition.Y;

            Position = new PixelPoint(
                Position.X + (int)deltaX,
                Position.Y + (int)deltaY
            );
            e.Handled = true;
        }
    }
    
    private void OnCardPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
            SnapToEdge();
            e.Handled = true;
        }
    }

    private void SnapToEdge()
    {
        var screen = Screens.Primary;
        if (screen == null) return;

        var workingArea = screen.WorkingArea;
        var threshold = 50; // 贴边吸附的阈值

        var newX = Position.X;
        var newY = Position.Y;

        // 检查左边缘
        if (Math.Abs(Position.X - workingArea.X) < threshold)
        {
            newX = workingArea.X;
        }
        // 检查右边缘
        else if (Math.Abs(Position.X + Width - (workingArea.X + workingArea.Width)) < threshold)
        {
            newX = workingArea.X + workingArea.Width - (int)Width;
        }

        // 检查上边缘
        if (Math.Abs(Position.Y - workingArea.Y) < threshold)
        {
            newY = workingArea.Y;
        }
        // 检查下边缘
        else if (Math.Abs(Position.Y + Height - (workingArea.Y + workingArea.Height)) < threshold)
        {
            newY = workingArea.Y + workingArea.Height - (int)Height;
        }

        Position = new PixelPoint(newX, newY);
        Opacity = 0.95;
    }


    public void ToggleVisibility()
    {
        if (IsVisible)
        {
            Hide();
        }
        else
        {
            Show();
            Activate();
        }
    }
}