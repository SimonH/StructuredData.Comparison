namespace StructuredData.Comparison.Interfaces
{
    public interface IPatchElement
    {
        string Operation { get; }
        string Path { get; }
        string Value { get; }
    }
}