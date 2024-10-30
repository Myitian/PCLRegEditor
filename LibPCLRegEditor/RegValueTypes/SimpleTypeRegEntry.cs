using Microsoft.Win32;

namespace LibPCLRegEditor.RegValueTypes;

public class SimpleTypeRegEntry<T>(string parent, string name) : PCLRegEntry(parent, name) where T : struct, IParsable<T>
{
    public T? Value { get; set; }

    public override void ReadValue()
    {
        if (T.TryParse(Registry.GetValue(Parent, Name, "") as string ?? "", null, out T val))
        {
            Value = val;
        }
        else
        {
            Value = null;
        }
    }

    public override string ToString()
    {
        return Value?.ToString() ?? "";
    }
}