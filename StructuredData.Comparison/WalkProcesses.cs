using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;
using StructuredData.Comparison.Processors;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison
{
    internal static class WalkProcesses
    {
        public static bool CheckForNulls(IEnumerable<IStructuredDataNode> sourceWalker, IEnumerable<IStructuredDataNode> resultWalker, List<PatchElement> patchElements)
        {
            if(sourceWalker == null)
            {
                var node = resultWalker?.FirstOrDefault();
                if(node != null)
                {
                    patchElements.Add(new PatchElement { Operation = "Add", Path = node.Path, Value = node.IsValue ? node.Value : null });
                }
                return false;
            }
            if(resultWalker == null)
            {
                var node = sourceWalker.FirstOrDefault();
                if(node != null)
                {
                    patchElements.Add(new PatchElement { Operation = "Remove", Path = node.Path });
                }
                return false;
            }
            return true;
        }

        public static Tuple<IStructuredDataNode, IStructuredDataNode> MoveNext(IEnumerator<IStructuredDataNode> sourceEnumerator, IEnumerator<IStructuredDataNode> resultEnumerator, Stack<ComparisonSettings> settingsScope, List<PatchElement> patchList)
        {
            var resultMoved = resultEnumerator.MoveNext();
            // process the settings when on the parent node
            if (resultMoved && resultEnumerator.Current.HasSettingsNode())
            {
                var retrievedSettings = resultEnumerator.Current.GetSettingsObject();
                if (retrievedSettings != null)
                {
                    settingsScope.Push(retrievedSettings);
                }
            }
            // skip over any actual settings that we hit here
            if (resultMoved && resultEnumerator.Current.IsSettingsNode())
            {
                resultMoved = resultEnumerator.MoveNext();
            }
            var sourceMoved = sourceEnumerator.MoveNext();
            if(resultMoved != sourceMoved)
            {
                if(resultMoved)
                {
                    patchList.Add(new PatchElement { Operation = "Add", Path = resultEnumerator.Current.Path, Value = resultEnumerator.Current.IsValue ? resultEnumerator.Current.Value : null });
                }
                else
                {
                    patchList.Add(new PatchElement { Operation = "Remove", Path = sourceEnumerator.Current.Path });
                }
                return null;
            }
            if(!resultMoved)
            {
                return null;
            }
            return new Tuple<IStructuredDataNode, IStructuredDataNode>(sourceEnumerator.Current, resultEnumerator.Current);
        }

        public static IEnumerable<IPatchElement> HandleLists(IStructuredDataNode sourceNode, IStructuredDataNode resultNode, Stack<ComparisonSettings> settingsScope)
        {
            var isStrict = (settingsScope.Peek().ListOptions & ListOptions.Strict) == ListOptions.Strict;
            var isOrdered = (settingsScope.Peek().ListOptions & ListOptions.Ordered) == ListOptions.Ordered;
            var areValues = (settingsScope.Peek().ListOptions & ListOptions.OfValues) == ListOptions.OfValues;
            var resultList = resultNode.Children.Where(sdn => !string.Equals(sdn.Name, ProcessorDeclarations.Settings)).ToList();
            var sourceList = sourceNode.Children.ToList();
            if (resultList.Count == 0)
            {
                if (sourceList.Count > 0 && isStrict)
                {
                    foreach (var node in sourceList)
                    {
                        yield return new PatchElement { Operation = "Remove", Path = node.Path };
                    }
                }
                yield break;
            }
            var keyField = settingsScope.Peek().ListKey ?? resultList[0].Children?.FirstOrDefault()?.Name;
            if (string.IsNullOrWhiteSpace(keyField))
            {
                throw new DataComparisonException("Cannot handle an unordered list without a key field. Set ListKey in settings or the first child node is used");
            }
            // we expect to find each result item and for them to be equal it's just where we find them in the source that's different
            //IListLocator listLocator = new ListL
            var listLocator = isOrdered ? (IListLocator)new OrderedListLocator(sourceList, keyField) : new UnOrderedListLocator(sourceList, keyField);
            List<IStructuredDataNode> foundNodes = new List<IStructuredDataNode>();
            foreach (var result in resultList)
            {
                var source = listLocator.Locate(result);
                if (source != null)
                {
                    foundNodes.Add(source);
                    foreach (var patch in HandleNodes(source, result, settingsScope))
                    {
                        yield return patch;
                    }
                }
                else
                {
                    yield return new PatchElement { Operation = "Add", Path = result.Path };
                }
            }
            if (isStrict)
            {
                // need to remove source nodes that weren't found here
                foreach (var source in sourceList)
                {
                    if (foundNodes.Contains(source))
                    {
                        continue;
                    }
                    yield return new PatchElement { Operation = "Remove", Path = source.Path };
                }
            }
        }

        public static IEnumerable<IPatchElement> HandleNodes(IStructuredDataNode sourceNode, IStructuredDataNode resultNode, Stack<ComparisonSettings> settingsScope)
        {
            if(!sourceNode.IsValue)
            {
                if(!resultNode.IsValue)
                {
                    foreach(var patch in StructuredDataComparer.Walk(sourceNode.Children, resultNode.Children, settingsScope))
                    {
                        yield return patch;
                    }
                    yield break;
                }

                foreach(var child in sourceNode.Children)
                {
                    yield return new PatchElement { Operation = "Remove", Path = child.Path };
                }
                yield return new PatchElement { Operation = "Add", Path = resultNode.Path, Value = resultNode.Value };
                yield break;
            }
            if(!resultNode.IsValue)
            {
                yield return new PatchElement { Operation = "Add", Path = resultNode.Path };
                yield return new PatchElement { Operation = "Remove", Path = sourceNode.Path };
                yield break;
            }
            if(!string.Equals(sourceNode.Name, resultNode.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                yield return new PatchElement { Operation = "Remove", Path = sourceNode.Path, Value = sourceNode.Value };
                yield return new PatchElement { Operation = "Add", Path = resultNode.Path, Value = resultNode.Value };
            }
            else if(!string.Equals(sourceNode.Value, resultNode.Value))
            {
                yield return new PatchElement { Operation = "Replace", Path = sourceNode.Path, Value = resultNode.Value };
            }
        }
    }
}