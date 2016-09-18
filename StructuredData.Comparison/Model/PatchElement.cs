using System;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.Model
{
    [Serializable]
    public class PatchElement : IPatchElement
    {
        public string Operation { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
    }
}