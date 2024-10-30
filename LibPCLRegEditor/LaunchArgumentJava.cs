using System.Text.Json.Serialization;

namespace LibPCLRegEditor;

public class LaunchArgumentJava
{
    private string path = "";
    private string versionString = "";

    [JsonPropertyName("Path")]
    public string Path { get => path; set => path = value ?? ""; }

    [JsonPropertyName("VersionString")]
    public string VersionString { get => versionString; set => versionString = value ?? ""; }

    [JsonPropertyName("IsJre")]
    public bool IsJre { get; set; }

    [JsonPropertyName("Is64Bit")]
    public bool Is64Bit { get; set; }

    [JsonPropertyName("IsUserImport")]
    public bool IsUserImport { get; set; }

    public new LaunchArgumentJava MemberwiseClone() => (LaunchArgumentJava)base.MemberwiseClone();
}
