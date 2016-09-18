using System.Collections.Generic;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.Model
{
    public class StructuredDataNode : IStructuredDataNode
    {
        public string Name { get; set; }
        public bool IsValue { get; set; }
        public string Value { get; set; }
        public IEnumerable<IStructuredDataNode> Children { get; set; }
        public string Path { get; set; }
    }
}