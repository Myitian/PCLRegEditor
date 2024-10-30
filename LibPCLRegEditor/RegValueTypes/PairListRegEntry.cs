using Microsoft.Win32;

namespace LibPCLRegEditor.RegValueTypes;

public class PairListRegEntry(string parent, string name) : PCLRegEntry(parent, name)
{
    public List<KeyValuePair<string, string>> Value { get; set; } = [];

    protected static KeyValuePair<string, string> InnerSplit(string item)
    {
        string[] sp = item.Split('>', 2);
        return sp.Length switch
        {
            0 => new("", ""),
            1 => new(sp[0], ""),
            _ => new(sp[0], sp[1]),
        };
    }

    public override void ReadValue()
    {
        Value = (Registry.GetValue(Parent, Name, "") as string ?? "").Split('|').Select(InnerSplit).ToList();
    }

    public override string ToString()
    {
        return string.Join('|', Value.Select(kvp => $"{kvp.Key}>{kvp.Value}"));
    }
}