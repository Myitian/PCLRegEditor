using Microsoft.Win32;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace LibPCLRegEditor.RegValueTypes;

public class JsonRegEntry<T>(string parent, string name, JsonTypeInfo<T> typeInfo) : PCLRegEntry(parent, name)
{
    public T? Value { get; set; } = default;
    public JsonTypeInfo<T> TypeInfo { get; set; } = typeInfo;

    public override void ReadValue()
    {
        if (Registry.GetValue(Parent, Name, "") is not string s)
        {
            Value = default;
        }
        else
        {
            Value = JsonSerializer.Deserialize(s, TypeInfo);
        }
    }

    public override string ToString()
    {
        return Value is null ? "null" : JsonSerializer.Serialize(Value, TypeInfo);
    }
}