using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison
{
    internal class OrderedListLocator : IListLocator
    {
        private readonly List<IStructuredDataNode> _findList;
        private readonly string _keyField;
        private int _currentIndex;
        public OrderedListLocator(List<IStructuredDataNode> findList, string keyField)
        {
            _findList = findList;
            _keyField = keyField;
        } 
        public IStructuredDataNode Locate(IStructuredDataNode nodeToLocate)
        {
            if (_findList == null || _findList.Count == 0 || _currentIndex == _findList.Count)
            {
                return null;
            }
            var keyValue = nodeToLocate?.Children?.FirstOrDefault(sdn => string.Equals(sdn.Name, _keyField, StringComparison.InvariantCultureIgnoreCase))?.Value;
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                return null;
            }
            while (_currentIndex < _findList.Count)
            {
                var node = _findList[_currentIndex];
                ++_currentIndex;
                if (node?.Children?.FirstOrDefault(sdn => string.Equals(sdn.Name, _keyField, StringComparison.InvariantCultureIgnoreCase) && string.Equals(sdn.Value, keyValue, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    return node;
                }
            }
            return null;
        }
    }
}