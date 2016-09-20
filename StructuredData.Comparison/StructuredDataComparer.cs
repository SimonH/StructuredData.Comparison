using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;
using StructuredData.Comparison.Processors;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison
{
    [Export(typeof(IStructuredDataComparer))]
    internal class StructuredDataComparer : IStructuredDataComparer
    {
        private IEnumerable<Lazy<ICreateStructuredDataWalkers, IFileMimeType>> _walkerFactories;
        private IEnumerable<Lazy<IConvertPatchElements, IFileMimeType>> _patchConverters;

        [ImportingConstructor]
        public StructuredDataComparer(
            [ImportMany] IEnumerable<Lazy<ICreateStructuredDataWalkers, IFileMimeType>> walkerFactories,
            [ImportMany] IEnumerable<Lazy<IConvertPatchElements, IFileMimeType>> patchConverters)
        {
            _walkerFactories = walkerFactories;
            _patchConverters = patchConverters;
        }

        public string Compare(string sourceData, string resultDeclarationData, string mimeType)
        {
            var walkerFactory =
                _walkerFactories.FirstOrDefault(
                        lwf => string.Equals(lwf.Metadata.MimeType, mimeType, StringComparison.CurrentCultureIgnoreCase))?
                    .Value;
            if (walkerFactory == null)
            {
                throw new DataComparisonException(
                    $"Could not locate an ICreateStructuredDataWalkers for mime type : {mimeType}");
            }
            var sourceWalker = walkerFactory.CreateWalker(sourceData);
            var resultWalker = walkerFactory.CreateWalker(resultDeclarationData);
            var settingsScope = new Stack<ComparisonSettings>(new List<ComparisonSettings> { new ComparisonSettings() });
            var differences = Walk(sourceWalker, resultWalker, settingsScope).ToList(); // to list here to prevent multiple enumerations
            return !differences.Any() ? null : CreatePatch(differences, mimeType);
        }

        private string CreatePatch(IEnumerable<IPatchElement> differences, string mimeType)
        {
            var converter = _patchConverters?.FirstOrDefault(pc => string.Equals(pc.Metadata.MimeType, mimeType, StringComparison.InvariantCultureIgnoreCase))?.Value ?? new DefaultXmlSerializerPatchConverter();
            return converter.DescribePatch(differences);
        }

        public static IEnumerable<IPatchElement> Walk(IEnumerable<IStructuredDataNode> sourceWalker,
            IEnumerable<IStructuredDataNode> resultWalker, Stack<ComparisonSettings> settingsScope)
        {
            var patchList = new List<PatchElement>();
            if(!WalkProcesses.CheckForNulls(sourceWalker, resultWalker, patchList))
            {
                foreach(var patch in patchList)
                {
                    yield return patch;
                }
                yield break;
            }
            var initialScopeDepth = settingsScope.Count;
            try
            {
                using(var resultEnumerator = resultWalker.GetEnumerator())
                {
                    // we should also start by checking that neither enumerator is empty (i.e. no foreach on results but GetEnumerator() and MoveNext() on each and see if it's true
                    using(var sourceEnumerator = sourceWalker.GetEnumerator())
                    {
                        while(true)
                        {
                            patchList.Clear();
                            var tuple = WalkProcesses.MoveNext(sourceEnumerator, resultEnumerator, settingsScope, patchList);
                            if(tuple == null)
                            {
                                foreach(var patch in patchList)
                                {
                                    yield return patch;
                                }
                                break;
                            }
                            var sourceNode = tuple.Item1;
                            var resultNode = tuple.Item2;
                            if(settingsScope.Count == initialScopeDepth && (!settingsScope.Peek().Inherit || resultNode.IsListNode()))
                            {
                                var lastInherited = settingsScope.LastInheritedSettings();
                                settingsScope.Push(!settingsScope.Peek().Inherit
                                    ? lastInherited
                                    : new ComparisonSettings
                                    {
                                        TreatAsList = true,
                                        Inherit = false,
                                        ListOptions = lastInherited.ListOptions,
                                        ListKey = lastInherited.ListKey
                                    });
                            }
                            IEnumerable<IPatchElement> commandPatches;
                            if(resultNode.RunValueProcessorCommand(sourceNode, out commandPatches))
                            {
                                foreach(var commandPatch in commandPatches)
                                {
                                    yield return commandPatch;
                                }
                                continue;
                            }
                            var currentSettings = settingsScope.Peek();
                            var handleFunc = currentSettings.TreatAsList ? new Func<IStructuredDataNode, IStructuredDataNode, Stack<ComparisonSettings>, IEnumerable<IPatchElement>>(WalkProcesses.HandleLists) : WalkProcesses.HandleNodes;
                            foreach(var patch in handleFunc(sourceNode, resultNode, settingsScope))
                            {
                                yield return patch;
                            }
                        }
                    }
                }
            }
            finally
            {
                if(settingsScope.Count > initialScopeDepth)
                {
                    settingsScope.Pop();
                }
            }
            
        }
    }
}