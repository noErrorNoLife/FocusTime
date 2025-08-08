# Avalonia 11.3.2 语法文档总结

## BoxShadow 属性语法

### 支持的控件
- Border: 直接支持 BoxShadow 属性
- ContentPresenter: 支持 BoxShadow 属性  
- Button: 通过模板中的 ContentPresenter 支持

### BoxShadow 语法格式
```
BoxShadow="offset-x offset-y blur-radius spread-radius color"
```

示例：
- `BoxShadow="5 5 10 0 DarkGray"`
- `BoxShadow="0 0 10 2 #BF000000"`
- `BoxShadow="0 2 8 0 #22000000"`

### Button 控件 BoxShadow 正确用法

**错误用法（Button 不直接支持 BoxShadow）:**
```xml
<Button BoxShadow="0 2 8 0 #22000000"/>  <!-- 错误 -->
```

**正确用法（通过样式设置模板中的 ContentPresenter）:**
```xml
<Style Selector="Button.icon /template/ ContentPresenter#PART_ContentPresenter">
    <Setter Property="BoxShadow" Value="0 2 8 0 #22000000"/>
</Style>
```

### 替代方案：用 Border 包装
```xml
<Border BoxShadow="0 2 8 0 #22000000" CornerRadius="25">
    <Button Classes="icon" Content="🎨"/>
</Border>
```

## UserControl 自定义属性

### 错误的绑定方式
```xml
<!-- 错误：Grid 没有 Progress 和 ProgressBrush 属性 -->
<Ellipse Stroke="{Binding $parent.ProgressBrush}"/>
<Ellipse Opacity="{Binding $parent.Progress}"/>
```

### 正确的自定义控件属性定义
```csharp
public partial class CircularProgressBar : UserControl
{
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<CircularProgressBar, double>(nameof(Progress), 0.0);

    public static readonly StyledProperty<IBrush> ProgressBrushProperty =
        AvaloniaProperty.Register<CircularProgressBar, IBrush>(nameof(ProgressBrush), Brushes.Blue);

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }
}
```

## 最佳实践

1. **Button 阴影**: 使用 Border 包装而不是直接设置 BoxShadow
2. **自定义控件**: 避免复杂的属性绑定，优先使用内置控件组合
3. **BoxShadow 语法**: 始终使用字符串格式 "x y blur spread color"
4. **模板样式**: 使用 `/template/` 选择器访问模板内部元素