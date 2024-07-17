using EpsilonLib.Shell.TreeModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.Cache;

namespace CacheEditor.Components.TagTree
{
    class TagTreeFolderView : ITagTreeViewMode
    {
        public IEnumerable<ITreeNode> BuildTree(GameCache cache, Func<CachedTag, bool> filter)
        {
            var tree = new List<ITreeNode>();
            var folderLookup = new Dictionary<string, TagTreeFolderNode>();

            var tags = cache.TagCache
                .NonNull()
                .Where(filter)
                .OrderByDescending(tag => tag.Name);

            foreach (var tag in tags)
                AddTag(tree, folderLookup, tag);

            SortNodes(tree);

            return tree;
        }

        private void AddTag(IList<ITreeNode> roots, Dictionary<string, TagTreeFolderNode> folderLookup, CachedTag tag)
        {
            if(tag.Name == null)
            {
                roots.Add(CreateTagNode(tag));
                return;
            }

            var segments = tag.Name.Split('\\');
            IList<ITreeNode> currentRoots = roots;

            for(int i = 0; i < segments.Length; i++)
            {
                if (i < segments.Length - 1)
                {
                    var folderKey = string.Join("\\", segments.Take(i + 1));

                    if (!folderLookup.TryGetValue(folderKey, out var node)) 
                    {
                        node = CreateFolderNode(segments[i]);
                        currentRoots.Add(node);
                        folderLookup[folderKey] = node;
                    }

                    currentRoots = node.Children;
                }
                else
                {
                    currentRoots.Add(CreateTagNode(tag));
                }
            }
        }

        private TagTreeFolderNode CreateFolderNode(string name)
        {
            return new TagTreeFolderNode() { Text = name };
        }

        private TagTreeTagNode CreateTagNode(CachedTag tag)
        {
            return new TagTreeTagNode(tag, () => FormatName(tag));
        }

        private string FormatName(CachedTag tag)
        {
            if (tag.Name == null)
            {
                return $"0x{tag.Index:X8}.{tag.Group}";
            }
            else
            {
                var fileName = Path.GetFileName(tag.Name);
                return $"{fileName}.{tag.Group}";
            }
        }

        private void SortNodes(IList<ITreeNode> nodes)
        {
            var folderNodes = nodes.OfType<TagTreeFolderNode>().OrderBy(n => n.Text).ToList();
            var tagNodes = nodes.OfType<TagTreeTagNode>().OrderBy(n => n.Text).ToList();

            nodes.Clear();
            foreach (var folderNode in folderNodes)
                nodes.Add(folderNode);

            foreach (var tagNode in tagNodes)
                nodes.Add(tagNode);

            foreach (var folderNode in folderNodes)
                SortNodes(folderNode.Children);
        }
    }

    public class TagTreeFolderNode : TagTreeNode { }
}
