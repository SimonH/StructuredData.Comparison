namespace StructuredData.Comparison.Settings
{
    internal class ComparisonSettings
    {
        public ComparisonSettings()
        {
            Inherit = true;
        }
        public bool TreatAsList { get; set; }
        public int ListOptions { get; set; }
        public bool Inherit { get; set; }

        public ComparisonSettings Clone()
        {
            return new ComparisonSettings
            {
                TreatAsList = this.TreatAsList,
                ListOptions = this.ListOptions,
                Inherit = this.Inherit
            };
        }
    }
}