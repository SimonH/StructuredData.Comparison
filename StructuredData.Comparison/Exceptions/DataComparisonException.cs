using System;
using System.Runtime.Serialization;

namespace StructuredData.Comparison.Exceptions
{
    [Serializable]
    public class DataComparisonException : Exception
    {
        public DataComparisonException()
        {
        }

        public DataComparisonException(string message) : base(message)
        {
        }

        public DataComparisonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataComparisonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}