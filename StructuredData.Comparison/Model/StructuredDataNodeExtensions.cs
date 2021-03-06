﻿using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Common.Container;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison.Model
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

        public static bool HasSettingsNode(this IStructuredDataNode resultNode)
        {
            return resultNode != null && !resultNode.IsValue && string.Equals(resultNode.Children?.FirstOrDefault()?.Name, ProcessorDeclarations.Settings);
        }

        public static bool IsListNode(this IStructuredDataNode resultNode)
        {
            if (resultNode == null || resultNode.IsValue)
                return false;

            var children = resultNode.Children.ToList();
            return children.Count > 1 && children.Where(sdn => !string.Equals(sdn.Name, ProcessorDeclarations.Settings, StringComparison.Ordinal)).All(sdn => string.Equals(sdn.Name, children[0].Name));
        }

        public static ComparisonSettings GetSettingsObject(this IStructuredDataNode resultNode)
        {
            var children = resultNode.Children.ToList();
            if(children.Count == 0)
            {
                return null;
            }
            // settings need to be on the first child of a node
            var settingsChild = children.FirstOrDefault(sdn => string.Equals(sdn.Name, ProcessorDeclarations.Settings));
            if (settingsChild == null)
            {
                return null;
            }
            var settingsChildren = settingsChild.Children.ToList();
            // By default inserted settings are not inherited, they are normally to describe list behaviour and do not refer to child nodes
            var ret = new ComparisonSettings
            {
                Inherit = string.Equals(settingsChildren.FirstOrDefault(sdn => string.Equals(sdn.Name, "Inherit"))?.Value, "true", StringComparison.OrdinalIgnoreCase),
                ListOptions = GetListOptions(settingsChildren.FirstOrDefault(sdn => string.Equals(sdn.Name, "ListOptions"))),
                ListKey = settingsChildren.FirstOrDefault(sdn => string.Equals(sdn.Name, "ListKey"))?.Value
            };
            // set treat as list implicitly if we have set list options or list key and then overwrite with any explicitly set value
            if (ret.ListOptions != ListOptions.LooseUnOrdered || !string.IsNullOrWhiteSpace(ret.ListKey))
            {
                ret.TreatAsList = true;
            }
            var treatAsListChild = settingsChildren.FirstOrDefault(sdn => string.Equals(sdn.Name, "TreatAsList"));
            if (treatAsListChild != null)
            {
                ret.TreatAsList = string.Equals(treatAsListChild.Value, "true", StringComparison.OrdinalIgnoreCase);
            }
            StringComparison newComparison;
            if (Enum.TryParse(settingsChildren.FirstOrDefault(sdn => string.Equals(sdn.Name, "StringComparison"))?.Value, true, out newComparison))
            {
                ret.StringComparison = newComparison;
            }
            return ret;
        }

        private static ListOptions GetListOptions(IStructuredDataNode node)
        {
            var ret = ListOptions.LooseUnOrdered;
            if(node?.Value == null)
            {
                return ret;
            }
            var parts = node.Value.Split(',');
            foreach(var part in parts)
            {
                ListOptions listOption;
                if (Enum.TryParse(part, true, out listOption))
                {
                    ret |= listOption;
                }
            }
            return ret;
        } 
    }
}