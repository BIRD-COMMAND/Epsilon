using EpsilonLib.Settings;

namespace CacheEditor.Components.TagTree
{
    public static class Settings
    {
        public static SettingDefinition TagTreeViewModeSetting =        new SettingDefinition("TagTreeViewMode",            ((int)TagTreeViewMode.Groups).ToString());
        public static SettingDefinition TagTreeGroupDisplaySetting =    new SettingDefinition("TagTreeGroupDisplayMode",    ((int)TagTreeGroupDisplayMode.TagGroupName).ToString());
        public static SettingDefinition ShowTagGroupAltNamesSetting =   new SettingDefinition("ShowTagGroupAltNames",       false.ToString());
        public static SettingDefinition BaseCacheWarningsSetting =      new SettingDefinition("BaseCacheWarnings",          true.ToString());
    }
}
