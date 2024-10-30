using Microsoft.Win32;

namespace LibPCLRegEditor.RegValueTypes;

public class StringRegEntry(string parent, string name) : PCLRegEntry(parent, name)
{
    public string Value { get; set; } = "";

    public override void ReadValue()
    {
        Value = Registry.GetValue(Parent, Name, "") as string ?? "";
    }

    public override string ToString()
    {
        return Value;
    }
}