using System.Security.Cryptography.X509Certificates;

namespace StructuredData.Comparison.Interfaces
{
    internal interface IStructuredDataComparer
    {
        string Compare(string sourceData, string resultDeclarationData, string mimeType);
    }
}