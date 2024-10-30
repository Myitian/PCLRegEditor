using LibPCLRegEditor;
using LibPCLRegEditor.RegValueTypes;
using Myitian.Controls;
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

internal class DoubleRegEntryEditorControl : NumericUpDown, IPCLRegEntryEditorControl<SimpleTypeRegEntry<double>>, INotifyPropertyChanged
{
    public static readonly DependencyProperty RegValueProperty =
        DependencyProperty.Register(
            "RegValue",
            typeof(SimpleTypeRegEntry<double>),
            typeof(DoubleRegEntryEditorControl),
            new UIPropertyMetadata(null));
    public static readonly DependencyProperty ControllerProperty =
        DependencyProperty.Register(
            "Controller",
            typeof(PCLRegEntryEditorController),
            typeof(DoubleRegEntryEditorControl),
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
    public SimpleTypeRegEntry<double>? RegValue
    {
        get => GetValue(RegValueProperty) as SimpleTypeRegEntry<double>;
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

    public DoubleRegEntryEditorControl():base()
    {
        OnValueChanged += OnChanged;
    }

    public bool changing = false;
    public void Reload()
    {
        changing = true;
        do
        {
            if (RegValue is null)
            {
                Value = 0;
                break;
            }
            RegValue.ReadValue();
            Value = RegValue.Value.HasValue ? (decimal)RegValue.Value : 0;
        } while (false);
        changing = false;
    }
    public void Flush()
    {
        if (RegValue is null)
            return;
        RegValue.Value = (double)Value;
        RegValue.WriteValue();
    }
    private void OnChanged(object? sender, EventArgs e)
    {
        if (!changing)
            Controller?.SetChanged();
    }
}
