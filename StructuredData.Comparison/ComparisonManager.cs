using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using MimeTypes;
using StructuredData.Common.interfaces;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison
{
    [Export(typeof(IManageStructuredDataComparison))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ComparisonManager : IManageStructuredDataComparison
    {
        private readonly IFileSystem _fileSystem;

        private readonly IStructuredDataComparer _comparisonTool;

        [ImportingConstructor]
        public  ComparisonManager(IFileSystem fileSystem, 
            IStructuredDataComparer comparisonTool)
        {
            _fileSystem = fileSystem;
            _comparisonTool = comparisonTool;
        }

        public string Compare(string sourceFilePath, string resultDeclarationFilePath)
        {
            if(!_fileSystem.Exists(sourceFilePath) || !_fileSystem.Exists(resultDeclarationFilePath))
            {
                throw new DataComparisonException("Cannot perform comparison. File does not exist");
            }
            var sourceMimeType = MimeTypeMap.GetMimeType(Path.GetExtension(sourceFilePath));
            var resultMimeType = MimeTypeMap.GetMimeType(Path.GetExtension(resultDeclarationFilePath));
            if(!string.Equals(sourceMimeType, resultMimeType, StringComparison.OrdinalIgnoreCase))
            {
                throw new DataComparisonException("Cannot compare files of different mime types");
            }
            return Compare(_fileSystem.ReadAllText(sourceFilePath), _fileSystem.ReadAllText(resultDeclarationFilePath), sourceMimeType);
        }

        public string Compare(string sourceData, string resultDeclarationData, string mimeType)
        {
            return _comparisonTool.Compare(sourceData, resultDeclarationData, mimeType);
        }
    }
}