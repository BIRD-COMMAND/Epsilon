namespace EpsilonLib.Settings
{
    public class SettingDefinition
    {
        public string Key { get; }
        public string DefaultValue { get; }

        public SettingDefinition(string key, string defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
        }
    }
}
