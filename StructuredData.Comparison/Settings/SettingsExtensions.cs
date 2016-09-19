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
    }
}