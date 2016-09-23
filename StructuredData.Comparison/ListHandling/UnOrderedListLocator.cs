using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    internal class UnOrderedListLocator : KeyedListLocatorBase
    {
        public UnOrderedListLocator(List<IStructuredDataNode> findList, string keyField, StringComparison comparison) : base(findList, keyField, comparison)
        {
        }

        protected override IStructuredDataNode LocateFromKey(string keyValue)
        {
            var node = SourceList.FirstOrDefault(sdn => 
                !FoundItemList.Contains(sdn) && sdn.Children?.FirstOrDefault(
                    child => string.Equals(child.Name, KeyField, Comparison) && string.Equals(child.Value, keyValue, Comparison)) != null);
            if (node != null)
            {
                FoundItemList.Add(node);
            }
            return node;
        }
    }
}