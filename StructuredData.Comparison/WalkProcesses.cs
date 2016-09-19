using System;
using System.Collections.Generic;
using System.Linq;
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
            if(resultMoved && resultEnumerator.Current.IsSettingsNode())
            {
                var retrievedSettings = resultEnumerator.Current.GetSettingsObject();
                if(retrievedSettings != null)
                {
                    settingsScope.Push(retrievedSettings);
                }
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

        public static IEnumerable<IPatchElement> HandleLists(IStructuredDataNode sourceNode, IStructuredDataNode resultNode, Stack<ComparisonSettings> settingsNode)
        {
            throw new NotImplementedException();
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