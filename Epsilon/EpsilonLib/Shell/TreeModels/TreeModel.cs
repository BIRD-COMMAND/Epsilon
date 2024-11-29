using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EpsilonLib.Shell.TreeModels
{
    public class TreeModel : PropertyChangedBase, ITreeViewEventSink
    {
        private ICollection<ITreeNode> _nodes;
        private ITreeNode _selectedNode;

		/// <summary>
		/// Event fired when the node is <i><b>selected</b></i> (highlighted). This may be independed of <see cref="NodeActivated"/>.
		/// </summary>
		public event EventHandler<TreeNodeEventArgs> NodeSelected;
		/// <summary>
		/// Event fired when the node is <i><b>activated</b></i>.<br/>
        /// Currently it is assumed that the node can only be activated while selected.
		/// </summary>
		public event EventHandler<TreeNodeEventArgs> NodeActivated;

        public ICollection<ITreeNode> Nodes
        {
            get => _nodes ?? (_nodes = new ObservableCollection<ITreeNode>());
            set => SetAndNotify(ref _nodes, value);
        }

        public ITreeNode SelectedNode
        {
            get => _selectedNode;
            set
            {
                var oldSelectedNode = _selectedNode;
                if (SetAndNotify(ref _selectedNode, value))
                {
                    if(oldSelectedNode != null)
                        oldSelectedNode.IsSelected = false;

                    if(_selectedNode != null)
                        _selectedNode.IsSelected = true;
                }  
            }
        }

        public IEnumerable<ITreeNode> FindNodesWithTag(object tag)
        {
            IEnumerable<ITreeNode> FindNodesWithTag(IEnumerable<ITreeNode> roots)
            {
                foreach(var node in roots)
                {
                    if (node.Tag == tag)
                        yield return node;

                    if (node.Children != null)
                    {
                        foreach (var child in FindNodesWithTag(node.Children))
                            yield return child;
                    }
                }
            }

            return FindNodesWithTag(_nodes);
        }


        #region ITreeEventSink Members

        object ITreeViewEventSink.Source { get; set; }
        
		void ITreeViewEventSink.NodeMouseActionAttempted(TreeNodeEventArgs e) {
            if (SelectedNode != null && SelectedNode == e.Node) { OnNodeActivated(e); }
		}

		void ITreeViewEventSink.NodeSelected(TreeNodeEventArgs e) {
			OnNodeSelected(e);
		}

        void ITreeViewEventSink.NodeKeyDown(TreeNodeEventArgs e) {
			if (SelectedNode == null || e.Node == null || ( SelectedNode != e.Node )) { return; }
			if (e.Key == Key.Enter) { OnNodeActivated(e); }
		}

		#endregion

		protected virtual void OnNodeSelected(TreeNodeEventArgs e)
        {
            SelectedNode = e.Node;
            NodeSelected?.Invoke(this, e);
        }

		protected virtual void OnNodeActivated(TreeNodeEventArgs e) {
            if (SelectedNode != null && SelectedNode != e.Node) { 
                OnNodeSelected(e); 
            }
			NodeActivated?.Invoke(this, e);
		}

	}
}
