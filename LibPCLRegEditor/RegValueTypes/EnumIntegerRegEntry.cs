using Microsoft.Win32;

namespace LibPCLRegEditor.RegValueTypes;

public class EnumRegEntry<TEnum>(string parent, string name) : PCLRegEntry(parent, name) where TEnum : struct, Enum
{
    public TEnum Value { get; set; }

    static EnumRegEntry()
    {
        Type t = typeof(TEnum);
        if (!t.IsEnum)
            throw new ArgumentException(t.Name + " 不是有效的枚举类型！", nameof(TEnum));
    }

    public override void ReadValue()
    {
        if (long.TryParse(Registry.GetValue(Parent, Name, "") as string ?? "", out long val))
            Value = (TEnum)Enum.ToObject(typeof(TEnum), val);
        else
            Value = default;
    }

    public override string ToString()
    {
        return Value.ToString("d");
    }
}