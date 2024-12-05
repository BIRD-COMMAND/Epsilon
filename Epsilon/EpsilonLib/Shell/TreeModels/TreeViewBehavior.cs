using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;

namespace EpsilonLib.Shell.TreeModels
{
    public static class TreeViewBehavior
    {
        public static bool GetBringIntoViewWhenSelected(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(BringIntoViewWhenSelectedProperty);
        }

        public static void SetBringIntoViewWhenSelected(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(BringIntoViewWhenSelectedProperty, value);
        }

		/// <summary>
		/// The BringIntoViewWhenSelected property is used to bring the TreeViewItem into view when it is selected.
		/// </summary>
		public static readonly DependencyProperty BringIntoViewWhenSelectedProperty =
			DependencyProperty.RegisterAttached("BringIntoViewWhenSelected", typeof(bool), typeof(TreeViewBehavior), new UIPropertyMetadata(false, OnBringIntoViewWhenSelectedChanged));

		/// <summary>
		/// This method is called when the BringIntoViewWhenSelected property is changed.<br/>
		/// It is used to bring the TreeViewItem into view when it is selected, but only if the new value is true.
		/// </summary>
		/// <param name="depObj"> The DependencyObject that the property is attached to. In practice, this is a TreeViewItem.</param>
		/// <param name="eventArgs"> The event arguments. Essentially the old and new values of the property.</param>
		static void OnBringIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs eventArgs) {
			if (depObj is TreeViewItem item && eventArgs.NewValue is bool newBoolValue && newBoolValue) { item.BringIntoView(); }
		}

		// In the broader context of WPF, a DependencyProperty is a property that is registered with the WPF property system.
		// This system allows for the property to be set in XAML, styled, data-bound, and animated.
		// Take, for instance, the Width property of a Button. This property is a DependencyProperty.
		// The DependencyProperty class is used to define a DependencyProperty, and it is used to register the property with the property system.
		// Once this is done, the property can be set in XAML, styled, data-bound, and animated.
		// Typically, on the XAML side of things, you'd see something that looks like this:
		// <Button Width="100" Height="50" Content="Click Me!" />
		// In this example, the Width property is set to 100. This is possible because the Width property is a DependencyProperty.
		// The DependencyProperty class has a RegisterAttached method that is used to register an attached property.
		// An attached property is a property that is defined by one class but attached to another class.

		// Let's more thoroughly review our particular use case:
		// The Model property is an attached property that is attached to the TreeView class.
		// The Model property is used to bind the TreeModel to the TreeView.
		// The Model property is registered as a DependencyProperty using the RegisterAttached method.
		// The RegisterAttached method takes four parameters: the name of the property, the type of the property, the type of the class that the property is attached to, and a PropertyMetadata object.
		// The PropertyMetadata object is used to specify a callback method that is called when the property is changed.
		// In this case, the OnModelChanged method is called when the Model property is changed.
		// The OnModelChanged method is used to bind the TreeModel to the TreeView.
		// The OnModelChanged method gets the TreeView from the DependencyObject and checks if the old value is an ITreeViewEventSink.
		// If the old value is an ITreeViewEventSink, it is disposed of.
		// The OnModelChanged method then checks if the new value is an ITreeViewEventSink.
		// If the new value is an ITreeViewEventSink, it is bound to the TreeView.
		// The TreeView is bound to the TreeModel's Nodes property.
		// The TreeView is attached to the TreeModel's events.

		// The key thing to realize here is that the DependencyProperty pattern is a fundamental part of WPF.
		// It allows properties to be set in XAML, styled, data-bound, and animated.
		// In this case, the Model property is used to bind the TreeModel to the TreeView.
		// This allows the TreeView to display the nodes of the TreeModel and respond to events from the TreeModel.

		public static object GetModel(DependencyObject obj)
        {
            return (object)obj.GetValue(ModelProperty);
        }

        public static void SetModel(DependencyObject obj, object value)
        {
            obj.SetValue(ModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached("Model", typeof(object), typeof(TreeViewBehavior), new PropertyMetadata(OnModelChanged));

		/// <summary>
		/// This method is called when the Model property is changed. It is used to bind the TreeModel to the TreeView.<br/>
		/// In detail, it binds the TreeModel to the TreeView's ItemsSource property.
		/// </summary>
		/// <param name="depObj"> The DependencyObject that the property is attached to. In practice, this is a TreeView.</param>
		/// <param name="eventArgs"> The event arguments. Essentially the old and new values of the property.</param>
		/// <remarks>
		/// Implementation Explanation:<br/>
		/// If the old value is an ITreeViewEventSink, it is disposed of.<br/>
		/// If the new value is an ITreeViewEventSink, it is bound to the TreeView.<br/>
		/// The TreeView is bound to the TreeModel's Nodes property.<br/>
		/// The TreeView is attached to the TreeModel's events.
		/// </remarks>
		private static void OnModelChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs eventArgs)
        {

			// Get the TreeView from the DependencyObject.
			if (depObj is TreeView treeView) {

				// If the old value is an ITreeViewEventSink, dispose of it.
				if (eventArgs.OldValue is ITreeViewEventSink oldSink) {
                    if (oldSink.Source is IDisposable disposable) { disposable.Dispose(); }
					oldSink.Source = null;
                }

				// If the new value is an ITreeViewEventSink, bind it to the TreeView.
				if (eventArgs.NewValue is ITreeViewEventSink newSink) {
					newSink.Source = new TreeViewModelBinding(treeView, newSink);
                }
            }
		}


		class TreeViewModelBinding : IDisposable
        {
            private readonly TreeView _treeView;
            private readonly ITreeViewEventSink _sink;
            private readonly Dictionary<RoutedEvent, RoutedEventHandler> _attachedHandlers;

            public TreeViewModelBinding(TreeView treeView, ITreeViewEventSink sink)
            {
                _sink = sink;
                _treeView = treeView;
                _attachedHandlers = new Dictionary<RoutedEvent, RoutedEventHandler>();

				_ = BindingOperations.SetBinding(
						_treeView, TreeView.ItemsSourceProperty,
						new Binding(nameof(TreeModel.Nodes)) { 
							Source = _sink 
						}
					);

				// The TreeViewItem.MouseDoubleClickEvent is associated with the TreeModel's NodeActionAttempted event.
				AttachHandler(TreeViewItem.MouseDoubleClickEvent, NodeDoubleClickHandler);
				// The TreeViewItem.SelectedEvent is associated with the TreeModel's NodeSelected event.
				AttachHandler(TreeViewItem.SelectedEvent, NodeSelectHandler);
				// The TreeViewItem.KeyDownEvent is associated with the TreeModel's KeyDown event.
				AttachHandler(UIElement.KeyDownEvent, NodeKeyDownHandler);
			
			}

            public void Dispose()
            {
				try {
					BindingOperations.ClearBinding(_treeView, TreeView.ItemsSourceProperty);
					foreach (KeyValuePair<RoutedEvent, RoutedEventHandler> pair in _attachedHandlers.ToList()) {
						try { DetachHandler(pair.Key, pair.Value); }
						catch { continue; }
					}
				}
				catch { }
			}


			/// <summary>
			/// Attaches a handler to a routed event.<br/>
			/// If the <paramref name="routedEvent"/> is null or the <paramref name="eventHandler"/> is null, an <see cref="Exception"/> is thrown.
			/// </summary>
			/// <param name="routedEvent"> The routed event to attach the handler to.</param>
			/// <param name="eventHandler"> The handler to attach to the routed event.</param>
			/// <exception cref="Exception"> Thrown when the <paramref name="routedEvent"/> is null or the <paramref name="eventHandler"/> is null.</exception>
			private void AttachHandler(RoutedEvent routedEvent, RoutedEventHandler eventHandler)
            {
				try {
					_attachedHandlers.Add(routedEvent, eventHandler);
					_treeView.AddHandler(routedEvent, eventHandler);
				}
				catch (Exception ex) {
					throw new Exception($"Failed to attach handler '{eventHandler?.ToString() ?? "NULL"}' for event '{routedEvent.Name ?? "NULL"}'.", ex);
				}
			}

			/// <summary>
			/// Detaches a handler from a routed event.<br/>
			/// If the <paramref name="routedEvent"/> is null or the <paramref name="eventHandler"/> is null, an <see cref="Exception"/> is thrown.
			/// </summary>
			/// <param name="routedEvent"> The routed event to detach the handler from.</param>
			/// <param name="eventHandler"> The handler to detach from the routed event.</param>
			/// <exception cref="Exception"> Thrown when the <paramref name="routedEvent"/> is null or the <paramref name="eventHandler"/> is null.</exception>
			private void DetachHandler(RoutedEvent routedEvent, RoutedEventHandler eventHandler)
            {
				try {
					_treeView.RemoveHandler(routedEvent, eventHandler);
					_ = _attachedHandlers.Remove(routedEvent);
				}
				catch (Exception ex) { 
					throw new Exception($"Failed to detach handler '{eventHandler?.ToString() ?? "NULL"}' for event '{routedEvent.Name ?? "NULL"}'.", ex); 
				}
			}


			private void NodeDoubleClickHandler(object sender, RoutedEventArgs e) {
				
				TreeViewItem treeViewItem = (e.OriginalSource as DependencyObject)
						.FindAncestors<TreeViewItem>().FirstOrDefault();

				if (treeViewItem?.IsMouseOver ?? false) {
					_sink.NodeMouseActionAttempted(
						new TreeNodeEventArgs(
							TreeViewItem.MouseDoubleClickEvent, sender, treeViewItem.DataContext as ITreeNode
						)
					);
				}

			}
			
			private void NodeSelectHandler(object sender, RoutedEventArgs e) {
				TreeViewItem treeViewItem = (e.OriginalSource as DependencyObject)
						.FindAncestors<TreeViewItem>().FirstOrDefault();
				if (treeViewItem != null) {
					_sink.NodeSelected(
						new TreeNodeEventArgs(
							TreeViewItem.SelectedEvent, sender, treeViewItem.DataContext as ITreeNode
						)
					);
				}
			}

			private void NodeKeyDownHandler(object sender, RoutedEventArgs e) {
				if (!(e is KeyEventArgs keyEventArgs)) { return; }
				TreeViewItem treeViewItem = (e.OriginalSource as DependencyObject)
						.FindAncestors<TreeViewItem>().FirstOrDefault();
				if (treeViewItem != null && treeViewItem.IsKeyboardFocusWithin && keyEventArgs.Key == Key.Enter) {
					_sink.NodeMouseActionAttempted(
						new TreeNodeEventArgs(TreeViewItem.KeyDownEvent, sender, treeViewItem.DataContext as ITreeNode, keyEventArgs.Key, Keyboard.Modifiers)
					);
				}
			}


		}
    }

	public class TreeNodeEventArgs : RoutedEventArgs
	{
		public ITreeNode Node { get; }
		public Key Key { get; } = Key.None;
		public ModifierKeys Modifiers { get; set; } = ModifierKeys.None;

		public TreeNodeEventArgs(RoutedEvent routedEvent, object source, ITreeNode node) : base(routedEvent, source) { Node = node; }
		public TreeNodeEventArgs(RoutedEvent routedEvent, object source, ITreeNode node, Key key, ModifierKeys modifiers) : base(routedEvent, source) { Node = node; Key = key; Modifiers = modifiers; }
	}

	public interface ITreeViewEventSink
	{
		object Source { get; set; }

		void NodeMouseActionAttempted(TreeNodeEventArgs e);
		void NodeSelected(TreeNodeEventArgs e);
		void NodeKeyDown(TreeNodeEventArgs e);

	}

}
