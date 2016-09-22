using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    internal class OrderedListOfValuesLocator : IListLocator
    {
        private readonly List<IStructuredDataNode> _findList;
        private int _currentIndex;
        public OrderedListOfValuesLocator(List<IStructuredDataNode> findList)
        {
            _findList = findList;
        }
        public IStructuredDataNode Locate(IStructuredDataNode nodeToLocate)
        {
            if(_findList == null || _findList.Count == 0 || _currentIndex == _findList.Count)
            {
                return null;
            }
            while(_currentIndex < _findList.Count)
            {
                var node = _findList[_currentIndex];
                ++_currentIndex;
                if(string.Equals(node?.Value, nodeToLocate.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return node;
                }
            }
            return null;
        }
    }
}