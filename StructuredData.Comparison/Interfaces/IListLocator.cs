using System.Collections.Generic;

namespace StructuredData.Comparison.Interfaces
{
    internal interface IListLocator
    {
        IStructuredDataNode Locate(IStructuredDataNode nodeToLocate);
        IEnumerable<IStructuredDataNode> FoundItems { get; }
    }
}