using System;

namespace StructuredData.Comparison.Settings
{
    [Flags]
    public enum ListOptions
    {
        LooseUnOrdered = 0,
        Strict = 1,
        Ordered = 2,
        OfValues = 4
    }
}