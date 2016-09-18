using System;
using System.Runtime.Serialization;

namespace StructuredData.Comparison.Exceptions
{
    [Serializable]
    public class FileComparisonException : Exception
    {
        public FileComparisonException()
        {
        }

        public FileComparisonException(string message) : base(message)
        {
        }

        public FileComparisonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileComparisonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}