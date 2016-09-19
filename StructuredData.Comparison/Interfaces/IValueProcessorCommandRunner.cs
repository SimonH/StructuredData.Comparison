using System.Collections.Generic;
using StructuredData.Comparison.Model;

namespace StructuredData.Comparison.Interfaces
{
    internal interface IValueProcessorCommandRunner
    {
        IEnumerable<IPatchElement> Run(string commmand, IStructuredDataNode sourceNode);
    }
}