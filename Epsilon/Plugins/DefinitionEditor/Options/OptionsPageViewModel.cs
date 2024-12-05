﻿using EpsilonLib.Options;
using EpsilonLib.Settings;
using System.ComponentModel.Composition;

namespace DefinitionEditor.Options
{
    [Export(typeof(IOptionsPage))]
    class OptionsPageViewModel : OptionPageBase
    {
        private readonly ISettingsCollection _settings;

        private bool _displayFieldTypes;
        private bool _displayFieldOffsets;
        private bool _collapseBlocks;

        [ImportingConstructor]
        public OptionsPageViewModel(ISettingsService settingsService) : base("Cache Editor", "Definition Editor")
        {
            _settings = settingsService.GetCollection(TagStructEditor.Settings.CollectionKey);
        }

        public bool DisplayFieldTypes
        {
            get => _displayFieldTypes;
            set => SetOptionAndNotify(ref _displayFieldTypes, value);
        }

        public bool DisplayFieldOffsets
        {
            get => _displayFieldOffsets;
            set => SetOptionAndNotify(ref _displayFieldOffsets, value);
        }

        public bool CollapseBlocks
        {
            get => _collapseBlocks;
            set =>  SetOptionAndNotify(ref _collapseBlocks, value);
        }

		public override void Save() { Apply(); }

		public override void Apply()
        {
            _settings.SetBool(TagStructEditor.Settings.DisplayFieldTypesSetting.Key, DisplayFieldTypes);
            _settings.SetBool(TagStructEditor.Settings.DisplayFieldOffsetsSetting.Key, DisplayFieldOffsets);
            _settings.SetBool(TagStructEditor.Settings.CollapseBlocksSetting.Key, CollapseBlocks);
        }

        public override void Load()
        {
            DisplayFieldTypes = _settings.GetBool(TagStructEditor.Settings.DisplayFieldTypesSetting);
            DisplayFieldOffsets = _settings.GetBool(TagStructEditor.Settings.DisplayFieldOffsetsSetting);
            CollapseBlocks = _settings.GetBool(TagStructEditor.Settings.CollapseBlocksSetting);
        }
    }
}
