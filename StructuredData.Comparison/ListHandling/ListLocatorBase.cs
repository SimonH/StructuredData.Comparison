using System;
using System.Collections.Generic;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    public abstract class ListLocatorBase : IListLocator
    {
        protected ListLocatorBase(List<IStructuredDataNode> sourceList, StringComparison comparison)
        {
            SourceList = sourceList;
            FoundItemList = new List<IStructuredDataNode>();
            Comparison = comparison;
        }

        public IStructuredDataNode Locate(IStructuredDataNode nodeToLocate)
        {
            if (SourceList == null || SourceList.Count == 0)
            {
                return null;
            }
            return LocateInternal(nodeToLocate);
        }

        protected abstract IStructuredDataNode LocateInternal(IStructuredDataNode nodeToLocate);
        protected StringComparison Comparison { get; }

        protected List<IStructuredDataNode> FoundItemList { get; private set; }
        public IEnumerable<IStructuredDataNode> FoundItems => FoundItemList;
        protected List<IStructuredDataNode> SourceList { get; }
    }
}