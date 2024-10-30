using LibPCLRegEditor;
using LibPCLRegEditor.RegValueTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PCLRegEditorGUI.WPF.Controls;

internal class BooleanRegEntryEditorControl : CheckBox, IPCLRegEntryEditorControl<SimpleTypeRegEntry<bool>>, INotifyPropertyChanged
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
            typeof(SimpleTypeRegEntry<bool>),
            typeof(BooleanRegEntryEditorControl),
            new UIPropertyMetadata(null));
    public static readonly DependencyProperty ControllerProperty =
        DependencyProperty.Register(
            "Controller",
            typeof(PCLRegEntryEditorController),
            typeof(BooleanRegEntryEditorControl),
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
    public SimpleTypeRegEntry<bool>? RegValue
    {
        get => GetValue(RegValueProperty) as SimpleTypeRegEntry<bool>;
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

    public BooleanRegEntryEditorControl() : base()
    {
        Checked += OnChanged;
        Unchecked += OnChanged;
    }

    private bool changing = false;
    public void Reload()
    {
        changing = true;
        do
        {
            if (RegValue is null)
            {
                IsChecked = false;
                break;
            }
            RegValue.ReadValue();
            IsChecked = RegValue.Value ?? false;
        } while (false);
        changing = false;
    }
    public void Flush()
    {
        if (RegValue is null)
            return;
        RegValue.Value = IsChecked ?? false;
        RegValue.WriteValue();
    }
    private void OnChanged(object? sender, EventArgs e)
    {
        if (!changing)
            Controller?.SetChanged();
    }
}
