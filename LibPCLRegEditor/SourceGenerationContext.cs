using System.Text.Json.Serialization;

namespace LibPCLRegEditor;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(List<LaunchArgumentJava>))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}