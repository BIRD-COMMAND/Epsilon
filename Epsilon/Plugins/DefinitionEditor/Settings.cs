using EpsilonLib.Settings;

namespace DefinitionEditor
{
	public static class Settings
	{
		public const string CollectionKey = "DefinitionEditor";

		public static SettingDefinition DisplayFieldTypesSetting = new SettingDefinition("DisplayFieldTypes", false.ToString());

		public static SettingDefinition DisplayFieldOffsetsSetting = new SettingDefinition("DisplayFieldOffsets", false.ToString());

		public static SettingDefinition CollapseBlocksSetting = new SettingDefinition("CollapseBlocks", false.ToString());
	}
}
