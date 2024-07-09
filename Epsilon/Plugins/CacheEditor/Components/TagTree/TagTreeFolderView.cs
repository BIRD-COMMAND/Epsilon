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

            var tags = cache.TagCache
                .NonNull()
                .Where(filter)
                .OrderByDescending(tag => tag.Name);

            foreach (var tag in tags)
                AddTag(tree, tag);

            return tree;
        }

        private void AddTag(IList<ITreeNode> roots, CachedTag tag)
        {
            if(tag.Name == null)
            {
                var node = CreateTagNode(tag);
                roots.Add(node);
                return;
            }

            var segments = tag.Name.Split('\\');
            for(int i = 0; i < segments.Length; i++)
            {
                if (i < segments.Length - 1)
                {
                    var node = FindNodeWithText(roots, segments[i]);
                    if(node == null)
                    {
                        node = CreateFolderNode(segments[i]);
                        roots.Insert(0, node);
                    }

                    roots = node.Children;
                }
                else
                {
                    var node = CreateTagNode(tag);
                    roots.Insert(0, node);
                }
            }

            SortNodes(roots);
        }

        private TagTreeNode FindNodeWithText(IList<ITreeNode> nodes, string text)
        {
            for (int j = 0; j < nodes.Count; j++)
                if (nodes[j] is TagTreeNode n && n.Text == text)
                    return n;

            return null;
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
            if(tag.Name == null)
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
        }
    }

    public class TagTreeFolderNode : TagTreeNode { }
}
