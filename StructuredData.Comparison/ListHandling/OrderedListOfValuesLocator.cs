using System;
using System.Collections.Generic;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    internal class OrderedListOfValuesLocator : ListLocatorBase
    {
        private int _currentIndex;
        public OrderedListOfValuesLocator(List<IStructuredDataNode> findList, StringComparison comparison) : base(findList, comparison)
        {
        }

        protected override IStructuredDataNode LocateInternal(IStructuredDataNode nodeToLocate)
        {
            if (_currentIndex == SourceList.Count)
            {
                return null;
            }
            while (_currentIndex < SourceList.Count)
            {
                var node = SourceList[_currentIndex++];
                if (FoundItemList.Contains(node) || !string.Equals(node?.Value, nodeToLocate.Value, Comparison))
                {
                    continue;
                }

                FoundItemList.Add(node);
                return node;
            }
            return null;
        }
    }
}