using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using StructuredData.Comparison.Interfaces;
using StructuredData.Comparison.Model;

namespace StructuredData.Comparison
{
    public class DefaultPatchConverter : IConvertPatchElements
    {
        public string DescribePatch(IEnumerable<IPatchElement> differences)
        {
            var list = differences.ToList();
            using(var writer = new StringWriter())
            {
                new XmlSerializer(typeof(PatchElement)).Serialize(writer, list);
                return writer.ToString();
            }
        }
    }
}