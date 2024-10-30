using LibPCLRegEditor.RegValueTypes;
using Microsoft.Win32;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LibPCLRegEditor;

public sealed class PCLRegEditor : IDictionary<string, PCLRegEntry>
{
    public const string DefaultPath = @"HKEY_CURRENT_USER\Software\PCL";
    private readonly Dictionary<string, PCLRegEntry> values = [];

    public PairListRegEntry? RegLaunchFolders => values.TryGetValue("LaunchFolders", out PCLRegEntry? value) ?
        value as PairListRegEntry :
        null;
    public SimpleTypeRegEntry<double>? RegWindowHeight => values.TryGetValue("WindowHeight", out PCLRegEntry? value) ?
        value as SimpleTypeRegEntry<double> :
        null;
    public SimpleTypeRegEntry<double>? RegWindowWidth => values.TryGetValue("WindowWidth", out PCLRegEntry? value) ?
        value as SimpleTypeRegEntry<double> :
        null;
    public StringRegEntry? RegCacheDownloadFolder => values.TryGetValue("CacheDownloadFolder", out PCLRegEntry? value) ?
        value as StringRegEntry :
        null;
    public JsonRegEntry<List<LaunchArgumentJava>>? RegLaunchArgumentJavaAll => values.TryGetValue("LaunchArgumentJavaAll", out PCLRegEntry? value) ?
        value as JsonRegEntry<List<LaunchArgumentJava>> :
        null;
    public EnumRegEntry<LoginType>? RegLoginType => values.TryGetValue("LoginType", out PCLRegEntry? value) ?
        value as EnumRegEntry<LoginType> :
        null;
    public SimpleTypeRegEntry<bool>? RegSystemEula => values.TryGetValue("SystemEula", out PCLRegEntry? value) ?
        value as SimpleTypeRegEntry<bool> :
        null;
    public SimpleTypeRegEntry<bool>? RegHintBuy => values.TryGetValue("HintBuy", out PCLRegEntry? value) ?
        value as SimpleTypeRegEntry<bool> :
        null;
    public SimpleTypeRegEntry<bool>? RegHintHandInstall => values.TryGetValue("HintHandInstall", out PCLRegEntry? value) ?
        value as SimpleTypeRegEntry<bool> :
        null;
    public SimpleTypeRegEntry<bool>? RegHintInstallBack => values.TryGetValue("HintInstallBack", out PCLRegEntry? value) ?
        value as SimpleTypeRegEntry<bool> :
        null;
    public SimpleTypeRegEntry<bool>? RegHintUpdateMod => values.TryGetValue("HintUpdateMod", out PCLRegEntry? value) ?
        value as SimpleTypeRegEntry<bool> :
        null;

    public string ParentKey { get; }
    public ICollection<string> Keys => values.Keys;
    public ICollection<PCLRegEntry> Values => values.Values;
    public int Count => values.Count;
    public bool IsReadOnly => false;

    public PCLRegEntry this[string name]
    {
        get => values.TryGetValue(name, out PCLRegEntry? entry) ? entry : throw new KeyNotFoundException();
        set => values[name] = value;
    }

    public PCLRegEditor(string? parent = DefaultPath)
    {
        parent ??= DefaultPath;
        ReadOnlySpan<char> pk = parent;
        int i = pk.IndexOf('\\');
        if (i <= 0)
        {
            throw new ArgumentException("错误的注册表路径", nameof(parent));
        }
        ReadOnlySpan<char> rootSpan = pk[..i];
        ReadOnlySpan<char> pathSpan = pk[(i + 1)..];
        rootSpan = rootSpan switch
        {
            "HKEY_CLASSES_ROOT" or "HKCR" => "HKEY_CLASSES_ROOT",
            "HKEY_CURRENT_CONFIG" => "HKEY_CURRENT_CONFIG",
            "HKEY_CURRENT_USER" or "HKCU" => "HKEY_CURRENT_USER",
            "HKEY_LOCAL_MACHINE" or "HKLM" => "HKEY_LOCAL_MACHINE",
            "HKEY_PERFORMANCE_DATA" => "HKEY_PERFORMANCE_DATA",
            "HKEY_USERS" or "HKU" => "HKEY_USERS",
            _ => throw new ArgumentException("错误的注册表路径", nameof(parent))
        };
        ParentKey = $"{rootSpan}\\{pathSpan}";
        Reload();
    }

    private RegistryKey? OpenKey()
    {
        ReadOnlySpan<char> pk = ParentKey;
        int i = pk.IndexOf('\\');
        ReadOnlySpan<char> rootSpan = pk[..i];
        ReadOnlySpan<char> pathSpan = pk[(i + 1)..];
        using RegistryKey root = rootSpan switch
        {
            "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
            "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
            "HKEY_CURRENT_USER" => Registry.CurrentUser,
            "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
            "HKEY_PERFORMANCE_DATA" => Registry.PerformanceData,
            "HKEY_USERS" => Registry.Users,
            _ => throw new Exception("错误的注册表路径")
        };
        return root.OpenSubKey(pathSpan.ToString());
    }

    public void FlushAndReload(bool removeNotExistedInThis = false)
    {
        FlushAll(removeNotExistedInThis);
        Reload();
    }
    public void Reload()
    {
        using RegistryKey? key = OpenKey();
        HashSet<string> updating = new(values.Keys);
        string[] names = key?.GetValueNames() ?? throw new Exception("注册表路径异常");
        foreach (string name in names)
        {
            if (!values.TryGetValue(name, out PCLRegEntry? regValue))
                values[name] = regValue = PCLRegEntry.CreateInstanceFromName(ParentKey, name);

            regValue.ReadValue();
            updating.Remove(name);
        }
        foreach (string name in updating)
        {
            values.Remove(name);
        }
    }
    public void Flush(string name)
    {
        if (values.TryGetValue(name, out PCLRegEntry? regValue))
            regValue.WriteValue();
    }
    public void FlushAll(bool removeNotExistedInThis = false)
    {
        foreach (var kvp in values)
            kvp.Value.WriteValue();
        if (!removeNotExistedInThis)
            return;

        using RegistryKey? key = OpenKey();
        HashSet<string> keys = new(values.Keys);
        string[] names = key?.GetValueNames() ?? throw new Exception("注册表路径异常");
        foreach (string name in names)
            if (!keys.Contains(name))
                key.DeleteSubKey(name);
    }
    public static void WriteEscapedString(TextWriter writer, string? str, bool? useHex = null)
    {
        const string HEX = "0123456789abcdef";
        ReadOnlySpan<char> span = str;
        useHex ??= span.ContainsAnyInRange('\x00', '\x1F');
        if (useHex.Value)
        {
            writer.Write("hex(1):");
            for (int i = 0; i < span.Length; i++)
            {
                int c = span[i];
                writer.Write(HEX[(c >> 4) & 0xF]);
                writer.Write(HEX[c & 0xF]);
                writer.Write(',');
                writer.Write(HEX[(c >> 12) & 0xF]);
                writer.Write(HEX[(c >> 8) & 0xF]);
                writer.Write(',');
            }
            writer.Write("00,00");
        }
        else
        {
            writer.Write('"');
            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];
                if (c is '\\' or '"')
                    writer.Write('\\');
                writer.Write(c);
            }
            writer.Write('"');
        }
    }
    public void Export(TextWriter writer)
    {
        writer.WriteLine("Windows Registry Editor Version 5.00");
        writer.WriteLine();
        writer.Write('[');
        writer.Write(ParentKey);
        writer.WriteLine(']');
        foreach (var kvp in values)
        {
            WriteEscapedString(writer, kvp.Key, false);
            writer.Write('=');
            WriteEscapedString(writer, kvp.Value.ToString());
            writer.WriteLine();
        }
        writer.WriteLine();
    }

    public PCLRegEntry? GetAndRemove(string name)
    {
        if (values.TryGetValue(name, out PCLRegEntry? value))
            values.Remove(name);
        else
            value = null;
        return value;
    }

    public bool ContainsKey(string key)
    {
        return values.ContainsKey(key);
    }
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out PCLRegEntry value)
    {
        return values.TryGetValue(key, out value);
    }
    public IEnumerator<KeyValuePair<string, PCLRegEntry>> GetEnumerator()
    {
        return values.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(string name, PCLRegEntry value)
    {
        values.Add(name, value);
    }
    public void Add(KeyValuePair<string, PCLRegEntry> item)
    {
        values.Add(item.Key, item.Value);
    }

    public bool Remove(string name)
    {
        return values.Remove(name);
    }
    public bool Remove(KeyValuePair<string, PCLRegEntry> item)
    {
        return TryGetValue(item.Key, out PCLRegEntry? value) && Equals(value, item.Value) && Remove(item.Key);
    }

    public void Clear()
    {
        values.Clear();
    }

    public bool Contains(KeyValuePair<string, PCLRegEntry> item)
    {
        return TryGetValue(item.Key, out PCLRegEntry? value) && Equals(value, item.Value);
    }

    public void CopyTo(KeyValuePair<string, PCLRegEntry>[] array, int arrayIndex)
    {
        foreach (var kvp in values)
        {
            array[arrayIndex++] = kvp;
        }
    }
}