using System.Collections.Generic;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison.ListHandling
{
    internal class ListLocatorFactory : IListLocatorFactory
    {
        public IListLocator CreateLocator(List<IStructuredDataNode> findList, ComparisonSettings currentSettings, string keyField)
        {
            return currentSettings.ListOptions.IsValueList() ? GetValueLocator(findList, currentSettings.ListOptions.IsOrdered()) : GetObjectLocator(findList, currentSettings.ListOptions.IsOrdered(), keyField);
        }

        private IListLocator GetObjectLocator(List<IStructuredDataNode> findList, bool isOrdered, string keyField)
        {
            return isOrdered ? (IListLocator)new OrderedListLocator(findList, keyField) : new UnOrderedListLocator(findList, keyField);
        }

        private IListLocator GetValueLocator(List<IStructuredDataNode> findList, bool isOrdered)
        {
            return isOrdered ? (IListLocator)new OrderedListOfValuesLocator(findList) : new UnorderedListOfValuesLocator(findList);
        }
    }
}