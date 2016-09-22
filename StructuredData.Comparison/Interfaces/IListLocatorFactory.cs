namespace StructuredData.Comparison.Interfaces
{
    internal interface IListLocatorFactory
    {
        IListLocator GetLocator(bool isOrdered, bool areValues);
    }
}