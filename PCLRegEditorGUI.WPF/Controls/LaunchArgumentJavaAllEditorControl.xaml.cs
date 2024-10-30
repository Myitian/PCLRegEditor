using LibPCLRegEditor;
using LibPCLRegEditor.RegValueTypes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PCLRegEditorGUI.WPF.Controls;

public partial class LaunchArgumentJavaAllEditorControl : UserControl, IPCLRegEntryEditorControl<JsonRegEntry<List<LaunchArgumentJava>>>, INotifyPropertyChanged
{
    /// <summary>
    /// 属性已改变
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// 属性已改变
    /// </summary>
    protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new(name));

    public static readonly DependencyProperty RegValueProperty =
        DependencyProperty.Register(
            "RegValue",
            typeof(JsonRegEntry<List<LaunchArgumentJava>>),
            typeof(LaunchArgumentJavaAllEditorControl),
            new UIPropertyMetadata(null));
    public static readonly DependencyProperty ControllerProperty =
        DependencyProperty.Register(
            "Controller",
            typeof(PCLRegEntryEditorController),
            typeof(LaunchArgumentJavaAllEditorControl),
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

    [Description("绑定的注册表值")]
    [Category("Common Properties")]
    /// <summary>绑定的注册表值</summary>
    public JsonRegEntry<List<LaunchArgumentJava>>? RegValue
    {
        get => GetValue(RegValueProperty) as JsonRegEntry<List<LaunchArgumentJava>>;
        set
        {
            SetValue(RegValueProperty, value);
            OnPropertyChanged(nameof(RegValue));
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
    private LaunchArgumentJava? editingObject;
    /// <summary>正在编辑的对象</summary>
    public LaunchArgumentJava? EditingObject
    {
        get => editingObject;
        set
        {
            editingObject = value;
            OnPropertyChanged(nameof(EditingObject));
        }
    }

    /// <summary>数据值</summary>
    public ObservableCollection<LaunchArgumentJava> Values { get; } = [];
    public LaunchArgumentJavaAllEditorControl() : base()
    {
        InitializeComponent();
        Values.CollectionChanged += OnChanged;
    }

    private bool changing = false;
    public void Reload()
    {
        changing = true;
        do
        {
            if (Values is null)
                break;
            if (RegValue is null)
            {
                Values.Clear();
                break;
            }
            RegValue.ReadValue();
            if (RegValue.Value is null)
            {
                Values.Clear();
                break;
            }
            Values.UpdateTo(RegValue.Value);
        } while (false);
        changing = false;
    }
    public void Flush()
    {
        if (RegValue is null)
            return;
        if (Values is null)
            return;
        RegValue.Value ??= [];
        RegValue.Value.UpdateTo(Values);
        RegValue.WriteValue();
    }
    private void OnChanged(object? sender, EventArgs e)
    {
        if (!changing)
            Controller?.SetChanged();
    }

    private bool preventUpdate = false;

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (EditingObject is null)
            return;
        Values?.Add(EditingObject.MemberwiseClone());
    }
    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        int i = LV_Main.SelectedIndex;
        if (i < 0)
            return;
        Values?.RemoveAt(i);
    }
    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        int i = LV_Main.SelectedIndex;
        if (i < 1 || Values is null)
            return;
        preventUpdate = true;
        int j = i - 1;
        (Values[i], Values[j]) = (Values[j], Values[i]);
        LV_Main.SelectedIndex = j;
        preventUpdate = false;
    }
    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        if (Values is null)
            return;
        int i = LV_Main.SelectedIndex;
        if (i < 0 || i > Values.Count - 2)
            return;
        preventUpdate = true;
        int j = i + 1;
        (Values[i], Values[j]) = (Values[j], Values[i]);
        LV_Main.SelectedIndex = j;
        preventUpdate = false;
    }
    private void Replace_Click(object sender, RoutedEventArgs e)
    {
        int i = LV_Main.SelectedIndex;
        if (i < 0 || Values is null || EditingObject is null)
            return;
        Values[i] = EditingObject.MemberwiseClone();
    }
    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int i = LV_Main.SelectedIndex;
        if (preventUpdate || i < 0 || Values is null)
            return;
        EditingObject = Values[i].MemberwiseClone();
    }
}
