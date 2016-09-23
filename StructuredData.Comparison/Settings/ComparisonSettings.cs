using System;

namespace StructuredData.Comparison.Settings
{
    internal class ComparisonSettings
    {
        public ComparisonSettings()
        {
            Inherit = true;
            StringComparison = StringComparison.OrdinalIgnoreCase;
        }
        public bool TreatAsList { get; set; }
        public ListOptions ListOptions { get; set; }
        public bool Inherit { get; set; }
        public string ListKey { get; set; }

        public StringComparison StringComparison { get; set; }

        public ComparisonSettings Clone()
        {
            return new ComparisonSettings
            {
                TreatAsList = this.TreatAsList,
                ListOptions = this.ListOptions,
                Inherit = this.Inherit,
                ListKey = this.ListKey,
                StringComparison = this.StringComparison
            };
        }
    }
}