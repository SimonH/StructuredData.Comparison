using NUnit.Framework;
using StructuredData.Common.interfaces;
using StructuredData.Comparison.Exceptions;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.Tests
{
    [TestFixture]
    public class ComparisonManagerFixture
    {
        private class MockFile : IFileSystem
        {
            public bool Exists(string fullPath)
            {
                return fullPath.StartsWith("exists");
            }

            public string ReadAllText(string fullPath)
            {
                return fullPath;
            }
        }

        private class MockComparer : IStructuredDataComparer
        {
            public string Compare(string sourceData, string resultDeclarationData, string mimeType)
            {
                return "compared";
            }
        }

        private MockFile _mockFile;
        private MockComparer _mockComparer;

        private ComparisonManager CreateSut()
        {
            return new ComparisonManager(_mockFile, _mockComparer);
        }

        [SetUp]
        public void SetUp()
        {
            _mockFile = new MockFile();
            _mockComparer = new MockComparer();
        }

        [Test]
        public void ThrowsIfEitherFileDoesNotExist()
        {
            var manager = CreateSut();
            Assert.That(() => manager.Compare("source.xml", "exists.xml"), Throws.TypeOf<DataComparisonException>());
            Assert.That(() => manager.Compare("exists.xml", "result.xml"), Throws.TypeOf<DataComparisonException>());
        }

        [Test]
        public void ThrowsIfMimeTypesDoNotMatch()
        {
            var manager = CreateSut();
            Assert.That(() => manager.Compare("exists.json", "exists.xml"), Throws.TypeOf<DataComparisonException>());
        }

        [Test]
        public void CallsComparerIfExists()
        {
            var manager = CreateSut();
            var candidate = manager.Compare("exists.xml", "exists.xml");
            Assert.That(candidate, Is.EqualTo("compared"));
        }
    }
}