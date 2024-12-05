using EpsilonLib.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagStructEditor
{
    public static class Settings
    {

		public const string CollectionKey = "TagResourceAndDefinitionEditorSettings";

		public static SettingDefinition DisplayFieldTypesSetting = new SettingDefinition("DisplayFieldTypes", false.ToString());
		public static SettingDefinition DisplayFieldOffsetsSetting = new SettingDefinition("DisplayFieldOffsets", false.ToString());
		public static SettingDefinition CollapseBlocksSetting = new SettingDefinition("CollapseBlocks", false.ToString());

		public static void Load(ISettingsService settingsService, TagStructEditor.Configuration config) {
			ISettingsCollection settings = settingsService.GetCollection(CollectionKey);
			config.DisplayFieldTypes = settings.GetBool(DisplayFieldTypesSetting);
			config.DisplayFieldOffsets = settings.GetBool(DisplayFieldOffsetsSetting);
			config.CollapseBlocks = settings.GetBool(CollapseBlocksSetting);
		}

	}
}
