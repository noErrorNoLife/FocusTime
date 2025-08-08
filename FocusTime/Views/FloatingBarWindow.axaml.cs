using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
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
            UpdateVisualState();
        };
        
        this.PointerExited += (s, e) => {
            _isPointerOver = false;
            UpdateVisualState();
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
                UpdateVisualState(); // Make sure the full bar is visible for dragging
            }

            _isDragging = true;
            _lastPosition = e.GetPosition(null); // Use screen coordinates
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }
    
    private void OnCardPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            var currentPosition = e.GetPosition(null);
            var delta = currentPosition - _lastPosition;
            _lastPosition = currentPosition;
            Position = new PixelPoint(Position.X + (int)delta.X, Position.Y + (int)delta.Y);
            e.Handled = true;
        }
    }
    
    private void OnCardPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
            // Defer the snap logic to avoid issues with the input event
            Dispatcher.UIThread.Post(SnapToEdge);
            e.Handled = true;
        }
    }

    private void SnapToEdge()
    {
        var screen = Screens.Primary;
        if (screen == null) return;

        var workingArea = screen.WorkingArea;
        var threshold = 30;

        // Check if we are near a horizontal edge
        if (Position.X < workingArea.X + threshold ||
            (Position.X + Width) > (workingArea.X + workingArea.Width - threshold))
        {
            _isSnapped = true;
        }
        else
        {
            _isSnapped = false;
        }

        // Always update the visual state after a drag release
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        var grid = this.FindControl<Grid>("MainGrid");
        if (grid == null) return;

        var screen = Screens.Primary;
        if (screen == null) return;
        var workingArea = screen.WorkingArea;

        bool shouldBeCollapsed = _isSnapped && !_isPointerOver;

        if (shouldBeCollapsed)
        {
            // --- COLLAPSED STATE ---
            grid.ColumnDefinitions[1].Width = new GridLength(0);
            grid.ColumnDefinitions[2].Width = new GridLength(0);

            // Calculate the width of the time text to adjust the visible part of the window
            var timeTextBlock = this.FindControl<TextBlock>("TimeDisplayTextBlock");
            double textWidth = 45; // Default value
            if (timeTextBlock != null && !string.IsNullOrEmpty(timeTextBlock.Text))
            {
                var formattedText = new FormattedText(
                    timeTextBlock.Text,
                    new Typeface(timeTextBlock.FontFamily, timeTextBlock.FontStyle, timeTextBlock.FontWeight),
                    timeTextBlock.FontSize,
                    TextAlignment.Left,
                    TextWrapping.NoWrap,
                    Size.Infinity
                );
                // Total visible width = text width + horizontal padding inside the card
                textWidth = formattedText.Width + 32; // 16px padding on each side
            }

            // Determine which edge we are snapped to and hide the window
            if (Position.X < workingArea.X + workingArea.Width / 2)
            {
                // Left edge
                Position = Position.WithX(workingArea.X - (int)Width + (int)textWidth);
            }
            else
            {
                // Right edge
                Position = Position.WithX(workingArea.X + workingArea.Width - (int)textWidth);
            }
            Opacity = 0.7;
        }
        else
        {
            // --- EXPANDED STATE ---
            grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);

            // If we are snapped, ensure we are fully visible
            if (_isSnapped)
            {
                if (Position.X < workingArea.X + workingArea.Width / 2)
                {
                    // Was snapped to the left, so show it fully on the left
                    Position = Position.WithX(workingArea.X);
                }
                else
                {
                    // Was snapped to the right, so show it fully on the right
                    Position = Position.WithX(workingArea.X + workingArea.Width - (int)Width);
                }
            }
            Opacity = 0.95;
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