using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;
using StructuredData.Comparison.Processors;

namespace StructuredData.Comparison
{
    [Export(typeof(IStructuredDataComparer))]
    internal class StructuredDataComparer : IStructuredDataComparer
    {
        private IEnumerable<Lazy<ICreateStructuredDataWalkers, IFileMimeType>> _walkerFactories;
        private IEnumerable<Lazy<IConvertPatchElements, IFileMimeType>> _patchConverters; 

        [ImportingConstructor]
        public StructuredDataComparer([ImportMany] IEnumerable<Lazy<ICreateStructuredDataWalkers, IFileMimeType>> walkerFactories,
            [ImportMany] IEnumerable<Lazy<IConvertPatchElements, IFileMimeType>> patchConverters)
        {
            _walkerFactories = walkerFactories;
        }

        public string Compare(string sourceData, string resultDeclarationData, string mimeType)
        {
            var walkerFactory = _walkerFactories.FirstOrDefault(lwf => string.Equals(lwf.Metadata.MimeType, mimeType, StringComparison.CurrentCultureIgnoreCase))?.Value;
            if(walkerFactory == null)
            {
                throw new DataComparisonException($"Could not locate an ICreateStructuredDataWalkers for mime type : {mimeType}");
            }
            var sourceWalker = walkerFactory.CreateWalker(sourceData);
            var resultWalker = walkerFactory.CreateWalker(resultDeclarationData);
            var differences = Walk(sourceWalker, resultWalker).ToList(); // to list here to prevent multiple enumerations
            return !differences.Any() ? null : CreatePatch(differences, mimeType);
        }

        private string CreatePatch(IEnumerable<IPatchElement> differences, string mimeType)
        {
            var converter = _patchConverters?.FirstOrDefault(pc => string.Equals(pc.Metadata.MimeType, mimeType, StringComparison.InvariantCultureIgnoreCase))?.Value ?? new DefaultXmlSerializerPatchConverter();
            return converter.DescribePatch(differences);
        }
        
        private IEnumerable<IPatchElement> Walk(IEnumerable<IStructuredDataNode> sourceWalker, IEnumerable<IStructuredDataNode> resultWalker)
        {
            if (sourceWalker == null)
            {
                var node = resultWalker?.FirstOrDefault();
                if (node != null)
                {
                    yield return new PatchElement {Operation = "Add", Path = node.Path, Value = node.IsValue ? node.Value : null };
                }
                yield break;
            }
            if (resultWalker == null)
            {
                var node = sourceWalker.FirstOrDefault();
                if (node != null)
                {
                    yield return new PatchElement { Operation = "Remove", Path = node.Path };
                }
                yield break;
            }
            using (var resultEnumerator = resultWalker.GetEnumerator())
            {
                // we should also start by checking that neither enumerator is empty (i.e. no foreach on results but GetEnumerator() and MoveNext() on each and see if it's true
                using (var sourceEnumerator = sourceWalker.GetEnumerator())
                {
                    while (true)
                    {
                        IStructuredDataNode resultNode = null;
                        IStructuredDataNode sourceNode = null;
                        var resultMoved = resultEnumerator.MoveNext();
                        var sourceMoved = sourceEnumerator.MoveNext();
                        if (resultMoved != sourceMoved)
                        {
                            if (resultMoved)
                            {
                                yield return new PatchElement { Operation = "Add", Path = resultEnumerator.Current.Path, Value = resultEnumerator.Current.IsValue ? resultEnumerator.Current.Value : null };
                            }
                            else
                            {
                                yield return new PatchElement { Operation = "Remove", Path = sourceEnumerator.Current.Path };
                            }
                            break;
                        }
                        if (!resultMoved)
                        {
                            break;
                        }
                        resultNode = resultEnumerator.Current;
                        sourceNode = sourceEnumerator.Current;
                        IEnumerable<IPatchElement> commandPatches;
                        if (resultNode.RunValueProcessorCommand(sourceNode, out commandPatches))
                        {
                            foreach (var commandPatch in commandPatches)
                            {
                                yield return commandPatch;
                            }
                            continue;
                        }
                        if (!sourceNode.IsValue)
                        {
                            if (!resultNode.IsValue)
                            {
                                foreach (var patch in Walk(sourceNode.Children, resultNode.Children))
                                {
                                    yield return patch;
                                }
                                continue;
                            }
                            if (resultNode.IsValue)
                            {
                                foreach (var child in sourceNode.Children)
                                {
                                    yield return new PatchElement {Operation = "Remove", Path = child.Path};
                                }
                                yield return new PatchElement { Operation = "Add", Path = resultNode.Path, Value = resultNode.Value };
                                continue;
                            }
                        }
                        if (!resultNode.IsValue)
                        {
                            yield return new PatchElement {Operation = "Add", Path = resultNode.Path};
                            yield return new PatchElement {Operation = "Remove", Path = sourceNode.Path};
                            continue;
                        }
                        if (
                            !string.Equals(sourceNode.Name, resultNode.Name,
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            yield return new PatchElement { Operation = "Remove", Path = sourceNode.Path, Value = sourceNode.Value };
                            yield return new PatchElement { Operation = "Add", Path = resultNode.Path, Value = resultNode.Value };
                        }
                        else if (!string.Equals(sourceNode.Value, resultNode.Value))
                        {
                            yield return new PatchElement { Operation = "Replace", Path = sourceNode.Path, Value = resultNode.Value };
                        }
                    }
                }
            }
        }
    }
}