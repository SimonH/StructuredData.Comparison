using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;

namespace StructuredData.Comparison.Processors
{
    [Export(typeof(IValueProcessorCommand))]
    [ExportMetadata("Name", "Ignore")]
    public class IgnoreValueProcessorCommand : IValueProcessorCommand
    {
        public IEnumerable<IPatchElement> Process(IStructuredDataNode sourceNode)
        {
            // Ignore doesn't do anything no patches just skip the node
            yield break;
        }
    }
}