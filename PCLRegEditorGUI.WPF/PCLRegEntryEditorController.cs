namespace PCLRegEditorGUI.WPF;

public class PCLRegEntryEditorController
{
    private bool isChanged = false;

    public event Action? ReloadAllEvent;
    public event Action? FlushAllEvent;
    public event Action<bool>? ChangedEvent;

    public bool IsChanged
    {
        get => isChanged;
        private set
        {
            isChanged = value;
            ChangedEvent?.Invoke(value);
        }
    }

    public void ReloadAll()
    {
        ReloadAllEvent?.Invoke();
        IsChanged = false;
    }

    public void FlushAll()
    {
        FlushAllEvent?.Invoke();
        IsChanged = false;
    }

    public void SetChanged()
    {
        IsChanged = true;
    }
}
