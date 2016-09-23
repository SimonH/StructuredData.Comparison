using System.Collections.Generic;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison.ListHandling
{
    internal class ListLocatorFactory : IListLocatorFactory
    {
        public IListLocator CreateLocator(List<IStructuredDataNode> findList, ComparisonSettings currentSettings, string keyField)
        {
            return currentSettings.ListOptions.IsValueList() ? GetValueLocator(findList, currentSettings) : GetObjectLocator(findList, currentSettings, keyField);
        }

        private IListLocator GetObjectLocator(List<IStructuredDataNode> findList, ComparisonSettings currentSettings, string keyField)
        {

            return currentSettings.ListOptions.IsOrdered() ? (IListLocator)new OrderedListLocator(findList, keyField, currentSettings.StringComparison) : new UnOrderedListLocator(findList, keyField, currentSettings.StringComparison);
        }

        private IListLocator GetValueLocator(List<IStructuredDataNode> findList, ComparisonSettings currentSettings)
        {
            return currentSettings.ListOptions.IsOrdered() ? (IListLocator)new OrderedListOfValuesLocator(findList, currentSettings.StringComparison) : new UnorderedListOfValuesLocator(findList, currentSettings.StringComparison);
        }
    }
}