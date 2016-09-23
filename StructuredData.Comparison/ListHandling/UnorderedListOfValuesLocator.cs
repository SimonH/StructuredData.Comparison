using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    internal class UnorderedListOfValuesLocator : ListLocatorBase
    {
        public UnorderedListOfValuesLocator(List<IStructuredDataNode> findList, StringComparison comparison) : base(findList, comparison)
        {
        }

        protected override IStructuredDataNode LocateInternal(IStructuredDataNode nodeToLocate)
        {
            var node = SourceList.FirstOrDefault(sdn => !FoundItemList.Contains(sdn) && string.Equals(sdn?.Value, nodeToLocate?.Value, Comparison));
            if (node != null)
            {
                FoundItemList.Add(node);
            }
            return node;
        }
    }
}