using System.Collections;
using System.Collections.Generic;
using StructuredData.Comparison.Model;

namespace StructuredData.Comparison.Interfaces
{
    public interface IValueProcessorCommand
    {
        IEnumerable<IPatchElement> Process(IStructuredDataNode sourceNode);
    }
}