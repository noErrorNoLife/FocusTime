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
    private bool _isSnapped = false;
    private bool _isPointerOver = false;

    public FloatingBarWindow()
    {
        InitializeComponent();
        this.Width = 300;
        
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

        this.PointerEntered += (s, e) => {
            _isPointerOver = true;
            UpdateVisualState(true);
        };
        
        this.PointerExited += (s, e) => {
            _isPointerOver = false;
            UpdateVisualState(true);
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
            if (_isSnapped)
            {
                _isSnapped = false;
                UpdateVisualState(false); // Make sure the full bar is visible for dragging
            }

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
        var threshold = 20; // A smaller threshold for snapping detection

        var currentPos = this.Position;
        var newPos = currentPos;
        bool isNowSnapped = false;

        // Check left edge
        if (Math.Abs(currentPos.X - workingArea.X) < threshold)
        {
            newPos = newPos.WithX(workingArea.X - (int)this.Width + 40);
            isNowSnapped = true;
        }
        // Check right edge
        else if (Math.Abs(currentPos.X + this.Width - (workingArea.X + workingArea.Width)) < threshold)
        {
            newPos = newPos.WithX(workingArea.X + workingArea.Width - 40);
            isNowSnapped = true;
        }

        // For top/bottom, we just snap flush without hiding
        if (Math.Abs(currentPos.Y - workingArea.Y) < threshold)
        {
            newPos = newPos.WithY(workingArea.Y);
        }
        else if (Math.Abs(currentPos.Y + this.Height - (workingArea.Y + workingArea.Height)) < threshold)
        {
            newPos = newPos.WithY(workingArea.Y + workingArea.Height - (int)this.Height);
        }

        _isSnapped = isNowSnapped;
        Position = newPos; // Set position before updating visual state
        UpdateVisualState(true);
    }

    private void UpdateVisualState(bool animated)
    {
        var progressPanel = this.FindControl<StackPanel>("ProgressAndPercentPanel");
        var buttonsPanel = this.FindControl<StackPanel>("ControlButtonsPanel");

        if (progressPanel == null || buttonsPanel == null) return;

        bool shouldBeCollapsed = _isSnapped && !_isPointerOver;

        if (shouldBeCollapsed)
        {
            // Collapse
            progressPanel.IsVisible = false;
            buttonsPanel.IsVisible = false;
            this.Width = 100; // Smaller width for collapsed view
            Opacity = 0.6;
        }
        else
        {
            // Expand
            progressPanel.IsVisible = true;
            buttonsPanel.IsVisible = true;
            this.Width = 300; // Full width
            Opacity = 0.95;

            // If we are snapped, we need to adjust position to be fully visible
            if (_isSnapped)
            {
                var screen = Screens.Primary;
                if (screen == null) return;
                var workingArea = screen.WorkingArea;

                if (Position.X < workingArea.X + workingArea.Width / 2)
                {
                    // Was snapped to the left
                    Position = Position.WithX(workingArea.X);
                }
                else
                {
                    // Was snapped to the right
                    Position = Position.WithX(workingArea.X + workingArea.Width - (int)this.Width);
                }
            }
        }
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