using EpsilonLib.Settings;

namespace Epsilon.Options
{
    public static class GeneralSettings
    {
        public const string CollectionKey = "General";

        public static SettingDefinition DefaultTagCache = new SettingDefinition("DefaultTagCache", string.Empty);
        public static SettingDefinition DefaultPak = new SettingDefinition("DefaultModPackage", string.Empty);
        public static SettingDefinition DefaultPakCache = new SettingDefinition("DefaultModPackageCache", string.Empty);
		public static SettingDefinition StartupPositionLeft = new SettingDefinition("StartupPositionLeft", "0");
        public static SettingDefinition StartupPositionTop = new SettingDefinition("StartupPositionTop", "0");
        public static SettingDefinition StartupWidth = new SettingDefinition("StartupWidth", "0");
        public static SettingDefinition StartupHeight = new SettingDefinition("StartupHeight", "0");
        public static SettingDefinition AlwaysOnTop = new SettingDefinition("AlwaysOnTop", false.ToString());
        public static SettingDefinition AccentColor = new SettingDefinition("AccentColor", "#007ACC");
    }
}
