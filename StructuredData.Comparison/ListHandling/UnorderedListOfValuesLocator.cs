using System;
using System.Collections.Generic;
using System.Linq;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.ListHandling
{
    internal class UnorderedListOfValuesLocator : IListLocator
    {
        private readonly List<IStructuredDataNode> _findList;
        public UnorderedListOfValuesLocator(List<IStructuredDataNode> findList)
        {
            _findList = findList;
        }

        public IStructuredDataNode Locate(IStructuredDataNode nodeToLocate)
        {
            if(_findList == null || _findList.Count == 0)
            {
                return null;
            }
            return _findList.FirstOrDefault(sdn => string.Equals(sdn?.Value, nodeToLocate?.Value, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}