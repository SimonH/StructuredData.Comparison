using System;
using System.Collections.Generic;
using System.Threading;

namespace StructuredData.Comparison.Settings
{
    internal static class SettingsExtensions
    {
        public static ComparisonSettings LastInheritedSettings(this Stack<ComparisonSettings> scopedSettings)
        {
            var array = scopedSettings.ToArray();
            var index = array.Length - 1;
            while(index >= 0)
            {
                if(array[index].Inherit)
                {
                    return array[index].Clone();
                }
                --index;
            }
            return new ComparisonSettings(); // shouldn't ever get here as the first element should always be inherited anyway
        }

        private static bool IsListOptionSet(this ListOptions source, ListOptions value)
        {
            return (source & value) == value;
        }

        public static bool IsValueList(this ListOptions source)
        {
            return source.IsListOptionSet(ListOptions.OfValues);
        }

        public static bool IsOrdered(this ListOptions source)
        {
            return source.IsListOptionSet(ListOptions.Ordered);
        }

        public static bool IsStrict(this ListOptions source)
        {
            return source.IsListOptionSet(ListOptions.Strict);
        }
    }
}