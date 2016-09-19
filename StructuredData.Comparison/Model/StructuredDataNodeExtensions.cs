using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Container;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison.Processors
{
    internal static class StructuredDataNodeExtensions
    {
        public static bool RunValueProcessorCommand(this IStructuredDataNode resultNode, IStructuredDataNode sourceNode, out IEnumerable<IPatchElement> patchElements)
        {
            patchElements = new List<IPatchElement>();
            if (resultNode.IsValue && resultNode.Value.StartsWith(ProcessorDeclarations.Prefix))
            {
                var runner = ContainerManager.CompositionContainer.GetExport<IValueProcessorCommandRunner>()?.Value;
                if (runner != null)
                {
                    patchElements = runner.Run(resultNode.Value.Substring(ProcessorDeclarations.Prefix.Length), sourceNode);
                    return true;
                }
            }
            return false;
        }

        public static bool IsSettingsNode(this IStructuredDataNode resultNode)
        {
            return resultNode != null && !resultNode.IsValue && string.Equals(resultNode.Name, ProcessorDeclarations.Settings);
        }

        public static bool IsListNode(this IStructuredDataNode resultNode)
        {
            if (resultNode == null || resultNode.IsValue)
                return false;

            var children = resultNode.Children.ToList();
            return children.Count > 1 && children.All(sdn => string.Equals(sdn.Name, children[0].Name));
        }

        public static ComparisonSettings GetSettingsObject(this IStructuredDataNode resultNode)
        {
            var children = resultNode.Children.ToList();
            if(children.Count == 0)
            {
                return null;
            }
            var ret = new ComparisonSettings
            {
                Inherit = string.Equals(children.FirstOrDefault(sdn => string.Equals(sdn.Name, "Inherit"))?.Value, "true", StringComparison.InvariantCultureIgnoreCase),
                TreatAsList = string.Equals(children.FirstOrDefault(sdn => string.Equals(sdn.Name, "Inherit"))?.Value, "true", StringComparison.InvariantCultureIgnoreCase),
                ListOptions = GetListOptions(children.FirstOrDefault(sdn => string.Equals(sdn.Name, "ListOptions")))
            };
            return ret;
        }

        private static int GetListOptions(IStructuredDataNode node)
        {
            var ret = ListOptions.LooseUnOrdered;
            if(node?.Value == null)
            {
                return (int)ret;
            }
            var parts = node.Value.Split(',');
            foreach(var part in parts)
            {
                if(string.Equals(part, "Strict", StringComparison.InvariantCultureIgnoreCase))
                {
                    ret |= ListOptions.Strict;
                }
                else if(string.Equals(part, "Ordered", StringComparison.InvariantCultureIgnoreCase))
                {
                    ret |= ListOptions.Ordered;
                }
            }
            return (int)ret;
        } 
    }
}