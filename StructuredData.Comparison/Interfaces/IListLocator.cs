namespace StructuredData.Comparison.Interfaces
{
    internal interface IListLocator
    {
        IStructuredDataNode Locate(IStructuredDataNode nodeToLocate);
    }
}