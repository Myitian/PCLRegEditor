using Microsoft.Win32;

namespace LibPCLRegEditor.RegValueTypes;

public class Base64BinaryRegEntry(string parent, string name) : PCLRegEntry(parent, name)
{
    public byte[] Value { get; set; } = [];

    public override void ReadValue()
    {
        Value = Convert.FromBase64String(Registry.GetValue(Parent, Name, "") as string ?? "");
    }

    public override string ToString()
    {
        return Convert.ToBase64String(Value);
    }
}