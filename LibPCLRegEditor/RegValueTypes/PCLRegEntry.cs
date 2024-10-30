using Microsoft.Win32;

namespace LibPCLRegEditor.RegValueTypes;

public abstract class PCLRegEntry
{
    public string Parent { get; }
    public string Name { get; }

    public PCLRegEntry(string parent, string name)
    {
        Parent = parent;
        Name = name;
        ReadValue();
    }

    public abstract void ReadValue();
    public virtual void WriteValue()
    {
        Registry.SetValue(Parent, Name, ToString() ?? "");
    }

    public static Type GetTypeFromName(string name)
    {
        return name switch
        {
            "CacheAuthAccess"
            or "CacheAuthClient"
            or "CacheAuthName"
            or "CacheAuthPass"
            or "CacheAuthServerName"
            or "CacheAuthServerRegister"
            or "CacheAuthServerServer"
            or "CacheAuthUuid"
            or "CacheAuthUsername"
            or "CacheMsAccess"
            or "CacheMsName"
            or "CacheMsOAuthRefresh"
            or "CacheMsProfileJson"
            or "CacheMsUuid"
            or "CacheNideServer"
            or "CacheNideAccess"
            or "LoginAuthEmail"
            or "LoginAuthPass"
            or "LoginLegacyName"
            or "LoginMsJson"
            or "SystemCount"
            or "SystemHighestBetaVersionReg"
            or "SystemLastVersionReg"
            or "SystemLaunchCount"
            or "UiLauncherThemeGold"
            or "UiLauncherThemeHide2" => typeof(Base64BinaryRegEntry),
            "LaunchFolders" => typeof(PairListRegEntry),
            "WindowHeight"
            or "WindowWidth" => typeof(SimpleTypeRegEntry<double>),
            "LoginType" => typeof(EnumRegEntry<LoginType>),
            "CacheJavaListVersion"
            or "HintDownload"
            or "HintNotice"
            or "SystemHelpVersion"
            or "ToolDownloadTranslate"
            or "ToolDownloadVersion" => typeof(SimpleTypeRegEntry<long>),
            "SystemEula"
            or "HintBuy"
            or "HintHandInstall"
            or "HintInstallBack"
            or "HintUpdateMod"
            or "HintModDisable"
            or "ToolDownloadKeepModpack"
            or "ToolUpdateRelease"
            or "ToolUpdateSnapshot" => typeof(SimpleTypeRegEntry<bool>),
            "LaunchArgumentJavaAll" => typeof(JsonRegEntry<List<LaunchArgumentJava>>),
            _ => typeof(StringRegEntry)
        };
    }
    public static PCLRegEntry CreateInstanceFromName(string parent, string name)
    {
        return name switch
        {
            "CacheAuthAccess"
            or "CacheAuthClient"
            or "CacheAuthName"
            or "CacheAuthPass"
            or "CacheAuthServerName"
            or "CacheAuthServerRegister"
            or "CacheAuthServerServer"
            or "CacheAuthUuid"
            or "CacheAuthUsername"
            or "CacheMsAccess"
            or "CacheMsName"
            or "CacheMsOAuthRefresh"
            or "CacheMsProfileJson"
            or "CacheMsUuid"
            or "CacheNideServer"
            or "CacheNideAccess"
            or "LoginAuthEmail"
            or "LoginAuthPass"
            or "LoginLegacyName"
            or "LoginMsJson"
            or "SystemCount"
            or "SystemHighestBetaVersionReg"
            or "SystemLastVersionReg"
            or "SystemLaunchCount"
            or "UiLauncherThemeGold"
            or "UiLauncherThemeHide2" => new Base64BinaryRegEntry(parent, name),
            "LaunchFolders" => new PairListRegEntry(parent, name),
            "WindowHeight"
            or "WindowWidth" => new SimpleTypeRegEntry<double>(parent, name),
            "LoginType" => new EnumRegEntry<LoginType>(parent, name),
            "CacheJavaListVersion"
            or "HintDownload"
            or "HintNotice"
            or "SystemHelpVersion"
            or "ToolDownloadTranslate"
            or "ToolDownloadVersion" => new SimpleTypeRegEntry<long>(parent, name),
            "SystemEula"
            or "HintBuy"
            or "HintHandInstall"
            or "HintInstallBack"
            or "HintUpdateMod"
            or "HintModDisable"
            or "ToolDownloadKeepModpack"
            or "ToolUpdateRelease"
            or "ToolUpdateSnapshot" => new SimpleTypeRegEntry<bool>(parent, name),
            "LaunchArgumentJavaAll" => new JsonRegEntry<List<LaunchArgumentJava>>(parent, name, SourceGenerationContext.Default.ListLaunchArgumentJava),
            _ => new StringRegEntry(parent, name)
        };
    }
}
