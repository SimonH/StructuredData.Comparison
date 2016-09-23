using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    internal class OrderedListLocator : KeyedListLocatorBase
    {
        private int _currentIndex;
        public OrderedListLocator(List<IStructuredDataNode> findList, string keyField, StringComparison comparison) : base(findList, keyField, comparison)
        {
        } 

        protected override IStructuredDataNode LocateFromKey(string keyValue)
        {
            if (_currentIndex == SourceList.Count)
            {
                return null;
            }
            while (_currentIndex < SourceList.Count)
            {
                var node = SourceList[_currentIndex++];
                if (FoundItems.Contains(node) ||
                    (node?.Children.FirstOrDefault(sdn => string.Equals(sdn.Name, KeyField, Comparison) && string.Equals(sdn.Value, keyValue, Comparison)) == null))
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