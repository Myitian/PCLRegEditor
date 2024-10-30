using LibPCLRegEditor.RegValueTypes;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PCLRegEditorGUI.WPF.Controls;

public abstract partial class EnumRegEntryEditorControl : UserControl, INotifyPropertyChanged
{
    /// <summary>
    /// 属性已改变
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// 属性已改变
    /// </summary>
    protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new(name));

    public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(EnumRegEntryEditorControl),
            new UIPropertyMetadata(false));
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(EnumRegEntryEditorControl),
            new UIPropertyMetadata("", (s, e) =>
            {
                if (s is not EnumRegEntryEditorControl control)
                    return;
                control.TextValueChanged?.Invoke(control, e);
            }));
    public static readonly DependencyProperty ControllerProperty =
        DependencyProperty.Register(
            "Controller",
            typeof(PCLRegEntryEditorController),
            typeof(EnumRegEntryEditorControl),
            new UIPropertyMetadata((s, e) =>
            {
                if (s is not IPCLRegEntryEditorControl control)
                    return;
                if (e.OldValue is PCLRegEntryEditorController controller)
                {
                    controller.ReloadAllEvent -= control.Reload;
                    controller.FlushAllEvent -= control.Flush;
                }
                if (e.NewValue is PCLRegEntryEditorController value)
                {
                    value.ReloadAllEvent += control.Reload;
                    value.FlushAllEvent += control.Flush;
                }
            }));

    [Description("是否可编辑")]
    [Category("Common Properties")]
    /// <summary>是否可编辑</summary>
    public bool IsEditable
    {
        get => (bool)GetValue(IsEditableProperty);
        set
        {
            SetValue(IsEditableProperty, value);
            OnPropertyChanged(nameof(IsEditable));
        }
    }
    [Description("文本")]
    [Category("Common Properties")]
    /// <summary>文本</summary>
    public string Text
    {
        get => GetValue(TextProperty) as string ?? "";
        set
        {
            SetValue(TextProperty, value ?? "");
            OnPropertyChanged(nameof(Text));
        }
    }
    [Description("事件控制器")]
    [Category("Common Properties")]
    /// <summary>事件控制器</summary>
    public PCLRegEntryEditorController? Controller
    {
        get => GetValue(ControllerProperty) as PCLRegEntryEditorController;
        set
        {
            SetValue(ControllerProperty, value);
            OnPropertyChanged(nameof(Controller));
        }
    }
    public abstract string[] Names { get; }
    public event Action<object?, DependencyPropertyChangedEventArgs>? TextValueChanged;
    public event RoutedEventHandler? ComboBoxLostFocus;

    protected EnumRegEntryEditorControl()
    {
        InitializeComponent();
    }

    private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
    {
        ComboBoxLostFocus?.Invoke(sender, e);
    }
}

public class EnumRegEntryEditorControl<TEnum> : EnumRegEntryEditorControl, IPCLRegEntryEditorControl<EnumRegEntry<TEnum>>
    where TEnum : struct, Enum
{
    public static readonly DependencyProperty RegValueProperty =
        DependencyProperty.Register(
            "RegValue",
            typeof(EnumRegEntry<TEnum>),
            typeof(EnumRegEntryEditorControl<TEnum>),
            new UIPropertyMetadata(null));
    public static readonly DependencyProperty EnumValueProperty =
        DependencyProperty.Register(
            "EnumValue",
            typeof(TEnum),
            typeof(EnumRegEntryEditorControl<TEnum>),
            new UIPropertyMetadata(default(TEnum), (s, e) =>
            {
                if (s is not EnumRegEntryEditorControl<TEnum> control)
                    return;
                control.OnEnumValueChanged(control, e);
            }));

    [Description("绑定的注册表值")]
    [Category("Common Properties")]
    /// <summary>绑定的注册表值</summary>
    public EnumRegEntry<TEnum>? RegValue
    {
        get => GetValue(RegValueProperty) as EnumRegEntry<TEnum>;
        set
        {
            SetValue(RegValueProperty, value);
            OnPropertyChanged(nameof(RegValue));
        }
    }
    [Description("枚举值")]
    [Category("Common Properties")]
    /// <summary>枚举值</summary>
    public TEnum EnumValue
    {
        get => (TEnum)GetValue(EnumValueProperty);
        set
        {
            SetValue(EnumValueProperty, value);
            OnPropertyChanged(nameof(EnumValue));
        }
    }

    public EnumRegEntryEditorControl() : base()
    {
        TextValueChanged += OnTextValueChanged;
        TextValueChanged += (_, _) => OnChanged(null, null);
        ComboBoxLostFocus += OnComboBoxLostFocus;
        OnPropertyChanged(nameof(Names));
    }

    private static readonly string[] names = Enum.GetNames<TEnum>();
    public override string[] Names => names;

    private bool changing = false;
    public void Reload()
    {
        changing = true;
        EnumValue = RegValue?.Value ?? default;
        changing = false;
    }
    public void Flush()
    {
        if (RegValue is null)
            return;
        RegValue.Value = EnumValue;
        RegValue.WriteValue();
    }
    private void OnChanged(object? sender, EventArgs? e)
    {
        if (!changing)
            Controller?.SetChanged();
    }

    private bool preventUpdate = false;
    private void OnTextValueChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (preventUpdate)
            return;
        preventUpdate = true;
        if (Enum.TryParse(e.NewValue as string, out TEnum result))
            EnumValue = result;
        preventUpdate = false;
    }
    private void OnEnumValueChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        if (preventUpdate)
            return;
        preventUpdate = true;
        TEnum oldValue = (TEnum)e.OldValue;
        TEnum newValue = (TEnum)e.NewValue;
        if (!oldValue.Equals(newValue))
            Text = newValue.ToString();
        preventUpdate = false;
    }
    private void OnComboBoxLostFocus(object sender, RoutedEventArgs e)
    {
        preventUpdate = true;
        if (!Enum.TryParse(Text, out TEnum result) || !result.Equals(EnumValue))
        {
            EnumValue = result;
            Text = result.ToString();
        }
        preventUpdate = false;
    }
}
