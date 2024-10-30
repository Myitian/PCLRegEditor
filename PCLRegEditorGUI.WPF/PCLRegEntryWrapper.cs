using LibPCLRegEditor.RegValueTypes;
using System.ComponentModel;

namespace PCLRegEditorGUI.WPF;

public class PCLRegEntryWrapper<T> : INotifyPropertyChanged where T : PCLRegEntry
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public T Value { get; }

    public PCLRegEntryWrapper(T value)
    {
        Value = value;
    }
}
