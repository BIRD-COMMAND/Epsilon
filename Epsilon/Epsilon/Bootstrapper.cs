using Epsilon;
using Epsilon.Options;
using Epsilon.Pages;
using EpsilonLib.Commands;
using EpsilonLib.Editors;
using EpsilonLib.Logging;
using EpsilonLib.Menus;
using EpsilonLib.Settings;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;


namespace WpfApp20
{
	public class Bootstrapper : MefBootstrapper<ShellViewModel>
	{
		private FileHistoryService _fileHistory;
		private IEditorService _editorService;
		private ISettingsCollection _settings;
		private string DefaultCachePath;
		private string DefaultPakPath;
		private string DefaultPakCachePath;
		private double StartupPositionLeft;
		private double StartupPositionTop;
		private double StartupWidth;
		private double StartupHeight;
		private bool AlwaysOnTop;
		private string AccentColor;

		protected async override void Launch() {
			RegisterAdditionalLoggers();

			List<Task> startupTasks = new List<Task>();
			startupTasks.Add(_fileHistory.InitAsync());

			PrepareResources();

			await Task.WhenAll(startupTasks);

			App.Current.DispatcherUnhandledException += UnhandledExceptionDisplay;

			List<IEditorProvider> providers = _editorService.EditorProviders.ToList();

			FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

			base.Launch();

			PostLaunchInitShell();

			await OpenDefault(providers.ElementAt(0), DefaultCachePath);
			await OpenDefault(providers.ElementAt(1), DefaultPakPath, DefaultPakCachePath);
		}

		private void RegisterAdditionalLoggers() {
			foreach (ILogHandler logger in GetInstances<ILogHandler>()) {
				Logger.RegisterLogger(logger);
			}
		}
		
		private void UnhandledExceptionDisplay(object sender, DispatcherUnhandledExceptionEventArgs args) {
			ExceptionDialog dialog = new ExceptionDialog(args.Exception);
			dialog.Owner = App.Current.MainWindow;
			dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			if (dialog.ShowDialog() == false) {
				args.Handled = true;
			}
		}

		protected override void ConfigureIoC(CompositionBatch batch) {
			base.ConfigureIoC(batch);

			_fileHistory = new FileHistoryService(new XmlFileHistoryStore("filehistory.xml"));
			batch.AddExportedValue<IFileHistoryService>(_fileHistory);
		}

		protected override IEnumerable<Assembly> GetAssemblies() {
			PluginLoader pluginManager = new PluginLoader();
			pluginManager.LoadPlugins();

			yield return Assembly.GetExecutingAssembly();
			yield return ( typeof(IShell).Assembly ); // EpsilonLib

			foreach (Host.PluginInfo file in pluginManager.Plugins) {
				yield return file.Assembly;
			}
		}

		private void PrepareResources() {
			foreach (ResourceDictionary dict in GetInstances<ResourceDictionary>()) {
				App.Current.Resources.MergedDictionaries.Add(dict);
			}

			_editorService = GetInstance<IEditorService>();
			_settings = GetInstance<ISettingsService>().GetCollection(GeneralSettings.CollectionKey);
			DefaultCachePath = _settings.Get(GeneralSettings.DefaultTagCache);
			DefaultPakPath = _settings.Get(GeneralSettings.DefaultPak);
			DefaultPakCachePath = _settings.Get(GeneralSettings.DefaultPakCache);
			AlwaysOnTop = _settings.GetBool(GeneralSettings.AlwaysOnTop);
			AccentColor = _settings.Get(GeneralSettings.AccentColor);

			App.Current.Resources.Add(typeof(ICommandRegistry), GetInstance<ICommandRegistry>());
			App.Current.Resources.Add(typeof(IMenuFactory), GetInstance<IMenuFactory>());
			App.Current.Resources.Add(SystemParameters.MenuPopupAnimationKey, PopupAnimation.None);
			App.Current.Resources[GeneralSettings.AlwaysOnTop.Key] = AlwaysOnTop;
		}

		private void PostLaunchInitShell() {

			// better font rendering

			TextOptions.TextFormattingModeProperty.OverrideMetadata(typeof(Window),
			   new FrameworkPropertyMetadata(TextFormattingMode.Display,
			   FrameworkPropertyMetadataOptions.AffectsMeasure |
			   FrameworkPropertyMetadataOptions.AffectsRender |
			   FrameworkPropertyMetadataOptions.Inherits));
			
			if (_settings.TryGetDouble(GeneralSettings.StartupPositionLeft, out StartupPositionLeft)
			&&  _settings.TryGetDouble(GeneralSettings.StartupPositionTop,  out StartupPositionTop)) {
				App.Current.MainWindow.Left = StartupPositionLeft;
				App.Current.MainWindow.Top = StartupPositionTop;
			}

			StartupWidth = _settings.GetDouble(GeneralSettings.StartupWidth);
			StartupHeight = _settings.GetDouble(GeneralSettings.StartupHeight);
			if (StartupWidth > 281 && StartupHeight > 500) {
				App.Current.MainWindow.Width = StartupWidth;
				App.Current.MainWindow.Height = StartupHeight;
			}

			InitAppearance();
		}

		private async Task OpenDefault(IEditorProvider editorProvider, params string[] paths) {
			if (paths == null || paths.Length == 0) { return; }
			string path = paths[0];
			if (string.IsNullOrWhiteSpace(path)) { return; }
			else if (File.Exists(path)) {
				if (paths.Length > 1) { await _editorService.OpenFileWithEditorAsync(editorProvider.Id, paths); }
				else { await _editorService.OpenFileWithEditorAsync(editorProvider.Id, path); }
			}
			else {
				MessageBox.Show($"Startup cache or mod package could not be found at the following location:" +
						$"\n\n{path}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void InitAppearance() {
			App.Current.Resources[GeneralSettings.AccentColor.Key] = (Color)ColorConverter.ConvertFromString(AccentColor);

			string epsilonTheme = "Default";
			Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary {
				Source = new Uri("/Epsilon;component/Themes/" + epsilonTheme.ToString() + ".xaml", UriKind.Relative)
			});
		}
	}
}
