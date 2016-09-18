namespace StructuredData.Comparison.Interfaces
{
    internal interface IManageStructuredDataComparison
    {
        string Compare(string sourceFilePath, string resultDeclarationFilePath);
        string Compare(string sourceData, string resultDeclarationData, string mimeType);
    }
}