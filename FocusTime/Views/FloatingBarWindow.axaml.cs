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
    private bool _isHidden = false;

    public FloatingBarWindow()
    {
        InitializeComponent();
        
        // 设置初始位置
        SetInitialPosition();
        
        // 添加拖动支持
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
        
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
        
        // 监听位置变化以实现贴边隐藏
        this.PositionChanged += OnPositionChanged;
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

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _lastPosition = e.GetPosition(this);
            e.Pointer.Capture(this);
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
            CheckEdgeHiding();
            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
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
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
            CheckEdgeHiding();
        }
    }

    private void OnPositionChanged(object? sender, PixelPointEventArgs e)
    {
        CheckEdgeHiding();
    }

    private void CheckEdgeHiding()
    {
        var screen = Screens.Primary;
        if (screen == null) return;

        var workingArea = screen.WorkingArea;
        var threshold = 10; // 贴边隐藏阈值

        bool shouldHide = false;

        // 检查是否贴近屏幕边缘
        if (Position.X <= workingArea.X + threshold ||
            Position.X >= workingArea.X + workingArea.Width - Width - threshold)
        {
            shouldHide = true;
        }

        if (shouldHide && !_isHidden)
        {
            HideToEdge();
        }
        else if (!shouldHide && _isHidden)
        {
            ShowFromEdge();
        }
    }

    private void HideToEdge()
    {
        _isHidden = true;
        var screen = Screens.Primary;
        if (screen == null) return;

        var workingArea = screen.WorkingArea;
        
        // 根据当前位置决定隐藏到哪边
        if (Position.X <= workingArea.X + workingArea.Width / 2)
        {
            // 隐藏到左边，只保留少量可见
            Position = new PixelPoint(workingArea.X - (int)Width + 15, Position.Y);
        }
        else
        {
            // 隐藏到右边，只保留少量可见
            Position = new PixelPoint(workingArea.X + workingArea.Width - 15, Position.Y);
        }

        // 可以添加动画效果
        Opacity = 0.6;
    }

    private void ShowFromEdge()
    {
        _isHidden = false;
        var screen = Screens.Primary;
        if (screen == null) return;

        var workingArea = screen.WorkingArea;
        
        // 从边缘完全显示出来
        if (Position.X < workingArea.X + 50)
        {
            Position = new PixelPoint(workingArea.X + 10, Position.Y);
        }
        else
        {
            Position = new PixelPoint(workingArea.X + workingArea.Width - (int)Width - 10, Position.Y);
        }

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