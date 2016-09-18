using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Wrappers;

namespace StructuredData.Comparison
{
    [Export(typeof(IManageStructuredDataComparison))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ComparisonManager : IManageStructuredDataComparison
    {
        private readonly IEnumerable<Lazy<IFileMimeType, IFileExtension>> _fileExtensionConversions;

        private readonly IFileSystem _fileSystem;

        private readonly IStructuredDataComparer _comparisonTool;

        [ImportingConstructor]
        public  ComparisonManager(IFileSystem fileSystem, 
            IStructuredDataComparer comparisonTool, 
            [ImportMany] IEnumerable<Lazy<IFileMimeType, IFileExtension>> fileExtensionConversions)
        {
            _fileSystem = fileSystem;
            _comparisonTool = comparisonTool;
            _fileExtensionConversions = fileExtensionConversions;
        }

        private string GetMimeType(string filePath)
        {
            if(!Path.HasExtension(filePath))
            {
                throw new FileComparisonException("files must have an extension");
            }

            var mimetype = _fileExtensionConversions.FirstOrDefault(fec => string.Equals(fec.Metadata.Extension, Path.GetExtension(filePath)?.Substring(1), StringComparison.InvariantCultureIgnoreCase))?.Value?.MimeType;
            if(string.IsNullOrWhiteSpace(mimetype))
            {
                throw new FileComparisonException($"could not retrieve mime type for file : {filePath}");
            }
            return mimetype;
        }

        public string Compare(string sourceFilePath, string resultDeclarationFilePath)
        {
            if(!_fileSystem.Exists(sourceFilePath) || !_fileSystem.Exists(resultDeclarationFilePath))
            {
                throw new FileComparisonException("Cannot perform comparison. File does not exist");
            }
            var sourceMimeType = GetMimeType(sourceFilePath);
            var resultMimeType = GetMimeType(resultDeclarationFilePath);
            if(!string.Equals(sourceMimeType, resultMimeType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new FileComparisonException("Cannot compare files of different mime types");
            }
            return Compare(_fileSystem.ReadAllText(sourceFilePath), _fileSystem.ReadAllText(resultDeclarationFilePath), sourceMimeType);
        }

        public string Compare(string sourceData, string resultDeclarationData, string mimeType)
        {
            return _comparisonTool.Compare(sourceData, resultDeclarationData, mimeType);
        }
    }
}