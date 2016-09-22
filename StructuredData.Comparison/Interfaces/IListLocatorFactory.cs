using System.Collections.Generic;
using StructuredData.Comparison.Settings;

namespace StructuredData.Comparison.Interfaces
{
    internal interface IListLocatorFactory
    {
        IListLocator CreateLocator(List<IStructuredDataNode> findList, ComparisonSettings currentSettings, string keyField);
    }
}