using LibPCLRegEditor;
using LibPCLRegEditor.RegValueTypes;
using Microsoft.Win32;
using Myitian.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PCLRegEditorGUI.WPF;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public const string RepoURL = "https://github.com/Myitian/PCLRegEditor";
    private static readonly ProcessStartInfo AboutPSI = new(RepoURL)
    {
        UseShellExecute = true
    };

    private PCLRegEditor? regKeys;
    private PCLRegEntryEditorController? controller;

    private readonly UnicodeEncoding UTF16LE = new(false, true, false);
    private readonly SaveFileDialog SFD = new()
    {
        Filter = "注册表文件|*.reg",
    };

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 属性已改变
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>
    /// 属性已改变
    /// </summary>
    protected void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new(name));

    public static readonly DependencyProperty BasicTitleProperty =
    DependencyProperty.Register(nameof(BasicTitle), typeof(string), typeof(MainWindow),
        new UIPropertyMetadata("", (s, e) =>
        {
            if (s is MainWindow window)
                window.OnPropertyChanged(nameof(TitleValue));
        }));
    [Description("Basic Title")]
    [Category("Common Properties")]
    public string BasicTitle
    {
        get => GetValue(BasicTitleProperty) as string ?? "";
        set
        {
            SetValue(BasicTitleProperty, value ?? "");
            OnPropertyChanged(nameof(BasicTitle));
        }
    }

    public string TitleValue => Controller?.IsChanged is true ? $"{BasicTitle} *" : BasicTitle;

    public PCLRegEditor? RegKeys
    {
        get => regKeys;
        set
        {
            regKeys = value;
            OnPropertyChanged(nameof(RegKeys));
        }
    }

    public PCLRegEntryEditorController? Controller
    {
        get => controller;
        set
        {
            controller = value;
            OnPropertyChanged(nameof(Controller));
        }
    }

    private void ControllerChanged(bool newState)
    {
        OnPropertyChanged(nameof(TitleValue));
    }
    private void Save()
    {
        Controller?.FlushAll();
        RegKeys?.FlushAndReload();
        Controller?.ReloadAll();
    }
    private void Reload()
    {
        RegKeys?.Reload();
        Controller?.ReloadAll();
    }
    private bool CheckChanged(string name)
    {
        if (Controller?.IsChanged == true)
        {
            switch (MessageBox.Show(string.Format("""
                有未保存的更改。是否要先保存更改？
                是：保存更改并{0}
                否：不保存更改并{0}
                取消：取消{0}
                """, name), name, MessageBoxButton.YesNoCancel))
            {
                case MessageBoxResult.Yes:
                    Save();
                    return true;
                case MessageBoxResult.No:
                    return true;
                default:
                    return false;
            }
        }
        return true;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Controller = new();
        Controller.ChangedEvent += ControllerChanged;
        RegKeys = [];
        Controller?.ReloadAll();
    }
    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (!CheckChanged("退出"))
            e.Cancel = true;
    }

    private void CMD_Save_Executed(object sender, ExecutedRoutedEventArgs? e)
    {
        Save();
    }
    private void CMD_Reload_Executed(object sender, ExecutedRoutedEventArgs? e)
    {
        if (!CheckChanged("重新加载"))
            return;
        Reload();
    }
    private void CMD_Export_Executed(object sender, ExecutedRoutedEventArgs? e)
    {
        if (RegKeys is null || !CheckChanged("导出备份"))
            return;
        if (SFD.ShowDialog() is true)
        {
            using StreamWriter sw = new(SFD.FileName, false, UTF16LE)
            {
                NewLine = "\r\n"
            };
            RegKeys.Export(sw);
            MessageBox.Show("导出成功！", "导出备份");
        }
    }
    private void CMD_About_Executed(object sender, ExecutedRoutedEventArgs? e)
    {
        AssemblyName name = Assembly.GetExecutingAssembly().GetName();
        if (MessageBox.Show($"""
            {name.Name} v{name.Version?.ToString(3)}
            作者：Myitian
            本程序与PCL/PCL2及其开发者没有任何从属关系，若出现问题请不要向PCL/PCL2及其开发者报告。

            代码仓库：{RepoURL}
            许可证：MIT
            按“确定”打开代码仓库链接
            """, "关于", MessageBoxButton.OKCancel) is MessageBoxResult.OK)
        {
            Process.Start(AboutPSI);
        }
    }
}