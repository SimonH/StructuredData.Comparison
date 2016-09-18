namespace StructuredData.Comparison.Interfaces
{
    internal interface IFileSystem
    {
        bool Exists(string fullPath);
        string ReadAllText(string fullPath);
    }
}