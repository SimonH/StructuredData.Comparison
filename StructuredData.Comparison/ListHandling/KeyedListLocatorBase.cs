using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    public abstract class KeyedListLocatorBase : ListLocatorBase
    {
        protected KeyedListLocatorBase(List<IStructuredDataNode> sourceList, string keyField, StringComparison comparison) : base(sourceList, comparison)
        {
            KeyField = keyField;
        }

        protected string KeyField { get; }

        private string GetKeyValue(IStructuredDataNode nodeToLocate)
        {
            var keyValue = nodeToLocate?.Children?.FirstOrDefault(sdn => string.Equals(sdn.Name, KeyField, Comparison))?.Value;
            return string.IsNullOrWhiteSpace(keyValue) ? null : keyValue;
        }

        protected sealed override IStructuredDataNode LocateInternal(IStructuredDataNode nodeToLocate)
        {
            var keyValue = GetKeyValue(nodeToLocate);
            return keyValue == null ? null : LocateFromKey(keyValue);
        }

        protected abstract IStructuredDataNode LocateFromKey(string keyValue);
    }
}