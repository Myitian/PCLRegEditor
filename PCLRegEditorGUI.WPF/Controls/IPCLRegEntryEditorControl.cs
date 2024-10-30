using LibPCLRegEditor.RegValueTypes;

namespace PCLRegEditorGUI.WPF.Controls;

public interface IPCLRegEntryEditorControl<T> : IPCLRegEntryEditorControl where T : PCLRegEntry
{
    T? RegValue { get; set; }
}
public interface IPCLRegEntryEditorControl
{
    PCLRegEntryEditorController? Controller { get; set; }
    void Reload();
    void Flush();
}
