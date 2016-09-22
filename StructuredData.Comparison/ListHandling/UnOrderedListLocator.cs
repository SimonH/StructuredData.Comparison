using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison
{
    internal class UnOrderedListLocator : IListLocator
    {
        private readonly List<IStructuredDataNode> _findList;
        private readonly string _keyField;
        public UnOrderedListLocator(List<IStructuredDataNode> findList, string keyField)
        {
            _findList = findList;
            _keyField = keyField;
        }

        public IStructuredDataNode Locate(IStructuredDataNode nodeToLocate)
        {
            if (_findList == null || _findList.Count == 0)
            {
                return null;
            }
            var keyValue = nodeToLocate?.Children?.FirstOrDefault(sdn => string.Equals(sdn.Name, _keyField, StringComparison.InvariantCultureIgnoreCase))?.Value;
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                return null;
            }
            return _findList.FirstOrDefault(
                sdn => sdn.Children?.FirstOrDefault(child => string.Equals(child.Name, _keyField, StringComparison.InvariantCultureIgnoreCase) && string.Equals(child.Value, keyValue, StringComparison.InvariantCultureIgnoreCase)) != null);
        }
    }
}