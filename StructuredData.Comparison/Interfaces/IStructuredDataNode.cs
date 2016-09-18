using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace StructuredData.Comparison.Interfaces
{
    public interface IStructuredDataNode
    {
        string Name { get; }
        bool IsValue { get; }
        string Value { get; }
        IEnumerable<IStructuredDataNode> Children { get; }
        string Path { get; }
    }
}