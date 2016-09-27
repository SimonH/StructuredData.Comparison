using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using NUnit.Framework;
using StructuredData.Common.Container;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison.Tests
{
    [TestFixture]
    public class StructuredDataComparisonFixture
    {
        private class ManageComparison : IManageStructuredDataComparison
        {
            public string ReturnValue { get; set; }
            public int CompareCalled { get; private set; }
            public string Compare(string sourceFilePath, string resultDeclarationFilePath)
            {
                CompareCalled++;
                return ReturnValue;
            }

            public string Compare(string sourceData, string resultDeclarationData, string mimeType)
            {
                CompareCalled++;
                return ReturnValue;
            }
        }

        private ManageComparison comparer;

        [SetUp]
        public void SetUp()
        {
            comparer = new ManageComparison { ReturnValue = "comparison" };
            var container = new CompositionContainer();
            container.ComposeExportedValue((IManageStructuredDataComparison)comparer);
            ContainerManager.SetExplicitContainer(container);
        }

        [Test]
        public void CompareWithFileInfo()
        {
            var candidate = new FileInfo("source").Comparison(new FileInfo("result"));
            Assert.That(candidate, Is.EqualTo("comparison"));
            Assert.That(comparer.CompareCalled, Is.EqualTo(1));
        }

        [Test]
        public void CompareWithFilePaths()
        {
            var candidate = "source".FileComparison("result");
            Assert.That(candidate, Is.EqualTo("comparison"));
            Assert.That(comparer.CompareCalled, Is.EqualTo(1));
        }

        [Test]
        public void CompareWithContents()
        {
            var candidate = "source".ContentComparison("result", "mime");
            Assert.That(candidate, Is.EqualTo("comparison"));
            Assert.That(comparer.CompareCalled, Is.EqualTo(1));
        }

        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("    ", true)]
        [TestCase("diff", false)]
        public void IsEqualToFileInfo(string comparison, bool expected)
        {
            comparer.ReturnValue = comparison;
            var candidate = new FileInfo("source").IsEqualTo(new FileInfo("result"));
            Assert.That(candidate, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("    ", true)]
        [TestCase("diff", false)]
        public void IsEqualToFilePaths(string comparison, bool expected)
        {
            comparer.ReturnValue = comparison;
            var candidate = "source".IsEqualTo("result");
            Assert.That(candidate, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("    ", true)]
        [TestCase("diff", false)]
        public void IsEqualToContents(string comparison, bool expected)
        {
            comparer.ReturnValue = comparison;
            var candidate = "source".IsEqualTo("result", "mime");
            Assert.That(candidate, Is.EqualTo(expected));
        }
    }
}