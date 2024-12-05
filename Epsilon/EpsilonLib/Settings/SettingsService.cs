using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Composition;
using System.IO;

namespace EpsilonLib.Settings
{
    [Export(typeof(ISettingsService))]
    class SettingsService : ISettingsService
    {
        private const string FilePath = "settings.json";
        private const string SettingsVersionKey = "SettingsVersion";
		private SettingsCollection _rootCollection;

        public event EventHandler<SettingChangedEventArgs> SettingChanged;

        public SettingsService()
        {
            _rootCollection = new SettingsCollection(this, new JObject());
            Load(FilePath);
        }

        public readonly static JsonSerializer Serializer = JsonSerializer.CreateDefault();
        static SettingsService()
        {
            Serializer.Formatting = Formatting.Indented;
            Serializer.Converters.Add(new StringEnumConverter());
        }

        public ISettingsCollection GetCollection(string key)
        {
            return _rootCollection.GetCollection(key);
        }

        private void Load(string filePath)
        {
            if (!File.Exists(filePath))
                return;

			using (JsonReader reader = new JsonTextReader(File.OpenText(filePath)))
            {
                reader.Read();
                _rootCollection = new SettingsCollection(this, JObject.ReadFrom(reader));
            }

			// 12/05/24 Breaking change to how settings are stored
			// If the Version key is not present we need to wipe the existing settings
			if (_rootCollection.Node[SettingsVersionKey] == null) {
				_rootCollection.Node.RemoveAll();               // Clear all existing data
				_rootCollection.Node[SettingsVersionKey] = 2;   // Record the update to V2
				Save(filePath);                                 // Save the changes
			}

		}

        private void Save(string filePath)
        {
            using (JsonWriter writer = new JsonTextWriter(File.CreateText(filePath)))
            {
                writer.Formatting = Formatting.Indented;
                _rootCollection.Node.WriteTo(writer);
            }
               
        }

        internal void NotifySettingChanged(SettingsCollection collection, string key)
        {
            Save(FilePath);
            SettingChanged?.Invoke(this, new SettingChangedEventArgs(collection, key));
        }
    }
}
