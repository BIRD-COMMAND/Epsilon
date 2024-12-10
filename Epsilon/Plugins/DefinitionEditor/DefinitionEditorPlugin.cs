using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using CacheEditor;
using CacheEditor.RTE;
using DefinitionEditor;
using EpsilonLib.Settings;
using Shared;
using TagStructEditor;
using TagStructEditor.Common;
using TagStructEditor.Fields;
using TagTool.Cache;

namespace DefinitionEditor
{
    [Export(typeof(ITagEditorPluginProvider))]
    class DefinitionEditorPluginProvider : ITagEditorPluginProvider
    {
        const string ContextKey = "DefinitionEditor.Context";

		private readonly IRteService _rteService;
		private readonly ISettingsCollection _settings;
        private readonly ISettingsService _settingsService;
		private readonly Lazy<IShell> _shell;
       
        [ImportingConstructor]
        public DefinitionEditorPluginProvider(Lazy<IShell> shell, ISettingsService settingsService, IRteService rteService)
        {
            _shell = shell;
            _settings = settingsService.GetCollection(TagStructEditor.Settings.CollectionKey);
			_settingsService = settingsService;
			_rteService = rteService;
		}

        public string DisplayName => "Definition";

        public int SortOrder => -1;

        public async Task<ITagEditorPlugin> CreateAsync(TagEditorContext context)
        {
			ValueChangedSink valueChangeSink = new ValueChangedSink();
			Configuration config = new TagStructEditor.Configuration()
            {
                OpenTag = context.CacheEditor.OpenTag,
                BrowseTag = context.CacheEditor.RunBrowseTagDialog,
                ValueChanged = valueChangeSink.Invoke,
				DisplayFieldTypes = _settings.GetBool(Settings.DisplayFieldTypesSetting.Key, false),
				DisplayFieldOffsets = _settings.GetBool(Settings.DisplayFieldOffsetsSetting.Key, false),
				CollapseBlocks = _settings.GetBool(Settings.CollapseBlocksSetting.Key, false)
			};
            TagStructEditor.Settings.Load(_settingsService, config);

			PerCacheDefinitionEditorContext ctx = GetDefinitionEditorContext(context);
			FieldFactory factory = new FieldFactory(ctx.Cache, ctx.TagList, config);
			object definitionData = await context.DefinitionData;
			StructField field = await Task.Run(() => CreateField(context, factory, definitionData));

            return new DefinitionEditorViewModel(
                _shell.Value,
				_rteService,
				context.CacheEditor,
                context.CacheEditor.CacheFile,
                context.Instance,
                definitionData,
                field,
                valueChangeSink,
                config);
        }

        private static StructField CreateField(TagEditorContext context, FieldFactory factory, object definitionData)
        {
			GameCache cache = context.CacheEditor.CacheFile.Cache;
			Type structType = cache.TagCache.TagDefinitions.GetTagDefinitionType(context.Instance.Group);

			Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

			StructField field = factory.CreateStruct(structType);
            Debug.WriteLine($"Create took {stopWatch.ElapsedMilliseconds}ms");

            stopWatch.Restart();

            field.Populate(definitionData);
            Debug.WriteLine($"Populate took {stopWatch.ElapsedMilliseconds}ms");

            return field;
        }

        private static PerCacheDefinitionEditorContext GetDefinitionEditorContext(TagEditorContext context)
        {
			ICacheEditor cacheEditor = context.CacheEditor;
			GameCache cache = cacheEditor.CacheFile.Cache;

            if (!cacheEditor.PluginStorage.TryGetValue(ContextKey, out object value) || 
                !ReferenceEquals(cache, (value as PerCacheDefinitionEditorContext).Cache))
            {
                value = new PerCacheDefinitionEditorContext(cache);
                context.CacheEditor.PluginStorage[ContextKey] = value;
            }

            return value as PerCacheDefinitionEditorContext;
        }

        public bool ValidForTag(ICacheFile cache, CachedTag tag)
        {
            return true;
        }

        class PerCacheDefinitionEditorContext
        {
            public GameCache Cache { get; }
            public TagList TagList { get; }

            public PerCacheDefinitionEditorContext(GameCache cache)
            {
                Cache = cache;
                TagList = new TagList(cache);
            }
        }

        class ValueChangedSink : IFieldsValueChangeSink
        {
            public event EventHandler<ValueChangedEventArgs> ValueChanged;

            public void Invoke(ValueChangedEventArgs e)
            {
                ValueChanged?.Invoke(this, e);
            }
        }
    }
}
