using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;

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
                throw new FileComparisonException($"Could not locate an ICreateStructuredDataWalkers for mime type : {mimeType}");
            }
            var sourceWalker = walkerFactory.CreateWalker(sourceData);
            var resultWalker = walkerFactory.CreateWalker(resultDeclarationData);
            var differences = Walk(sourceWalker, resultWalker);
            return !differences.Any() ? null : CreatePatch(differences, mimeType);
        }

        private string CreatePatch(IEnumerable<IPatchElement> differences, string mimeType)
        {
            var converter = _patchConverters.FirstOrDefault(pc => string.Equals(pc.Metadata.MimeType, mimeType, StringComparison.InvariantCultureIgnoreCase))?.Value ?? new DefaultPatchConverter();
            return converter.DescribePatch(differences);
        } 

        private IEnumerable<IPatchElement> Walk(IEnumerable<IStructuredDataNode> sourceWalker, IEnumerable<IStructuredDataNode> resultWalker)
        {
            using(var sourceEnumerator = sourceWalker.GetEnumerator())
            {
                foreach(var resultNode in resultWalker)
                {
                    if(sourceEnumerator.MoveNext())
                    {
                        var sourceNode = sourceEnumerator.Current;
                        if(!sourceNode.IsValue && resultNode.IsValue)
                        {
                            throw new NotImplementedException("Currently only single level objects are supported");
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
        }
    }
}