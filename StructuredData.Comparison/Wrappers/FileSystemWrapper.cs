using System.ComponentModel.Composition;
using System.IO;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.Wrappers
{
    [Export(typeof(IFileSystem))]
    internal class FileSystemWrapper : IFileSystem
    {
        public bool Exists(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}