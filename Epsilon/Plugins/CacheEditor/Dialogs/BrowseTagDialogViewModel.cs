﻿using CacheEditor.Components.TagTree;
using EpsilonLib.Shell.TreeModels;
using Stylet;
using TagTool.Cache;

namespace CacheEditor
{
    class BrowseTagDialogViewModel : Screen
    {
        public TagTreeViewModel TagTree { get; }

        public BrowseTagDialogViewModel(ICacheEditingService cacheEditingService, ICacheFile cacheFile)
        {
            TagTree = new TagTreeViewModel(cacheEditingService, cacheFile);
            TagTree.NodeActivated += TagTree_NodeActivated;
            DisplayName = "Tag Browser";
        }

        private void TagTree_NodeActivated(object sender, TreeNodeEventArgs e)
        {
            if(e.Node.Tag is CachedTag)
                RequestClose(true);
        }
    }
}