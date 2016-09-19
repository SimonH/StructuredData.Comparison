using System;
using System.Collections.Generic;
using StructuredData.Comparison.Container;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison.Processors
{
    public static class StructuredDataNodeExtensions
    {
        public static bool RunValueProcessorCommand(this IStructuredDataNode resultNode, IStructuredDataNode sourceNode, out IEnumerable<IPatchElement> patchElements)
        {
            patchElements = new List<IPatchElement>();
            if (resultNode.IsValue && resultNode.Value.StartsWith(ValueProcessorDeclarations.Prefix))
            {
                var runner = ContainerManager.CompositionContainer.GetExport<IValueProcessorCommandRunner>()?.Value;
                if (runner != null)
                {
                    patchElements = runner.Run(resultNode.Value.Substring(ValueProcessorDeclarations.Prefix.Length), sourceNode);
                    return true;
                }
            }
            return false;
        }
    }
}