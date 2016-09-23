using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Xml.XPath;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;

namespace StructuredData.Comparison.Processors
{
    [Export(typeof(IValueProcessorCommandRunner))]
    internal class ValueProcessorCommandRunner : IValueProcessorCommandRunner
    {
        IEnumerable<Lazy<IValueProcessorCommand, IValueProcessorCommandName>> _valueProcessorCommands;

        [ImportingConstructor]
        public ValueProcessorCommandRunner(
            [ImportMany] IEnumerable<Lazy<IValueProcessorCommand, IValueProcessorCommandName>> valueProcessorCommands)
        {
            _valueProcessorCommands = valueProcessorCommands;
        }

        public IEnumerable<IPatchElement> Run(string command, IStructuredDataNode sourceNode)
        {
            var commandProcessor = _valueProcessorCommands?.FirstOrDefault(pc => string.Equals(pc.Metadata.Name, command, StringComparison.OrdinalIgnoreCase))?.Value;
            if (commandProcessor == null)
            {
                throw new DataComparisonException($"No processor found for value processor : {command}");
            }
            return commandProcessor.Process(sourceNode);
        }
    }
}