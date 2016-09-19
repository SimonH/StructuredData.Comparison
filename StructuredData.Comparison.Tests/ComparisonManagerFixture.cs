using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Moq;
using NUnit.Framework;
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

        private class MockExtensionExt : IFileExtension
        {
            public string Extension => "ext";
        }

        private class MockMimeExt : IFileMimeType
        {
            public string MimeType => "mimetypeext";
        }

        private class MockExtensionEnt : IFileExtension
        {
            public string Extension => "ent";
        }

        private class MockMimeEnt : IFileMimeType
        {
            public string MimeType => "mimetypeent";
        }

        private MockFile mockFile;
        private MockComparer mockComparer;
        private List<Lazy<IFileMimeType, IFileExtension>> lazyList; 

        private ComparisonManager CreateSut()
        {
            return new ComparisonManager(mockFile, mockComparer, lazyList);
        }

        [SetUp]
        public void SetUp()
        {
            mockFile = new MockFile();
            mockComparer = new MockComparer();
            lazyList = new List<Lazy<IFileMimeType, IFileExtension>>
            {
                new Lazy<IFileMimeType, IFileExtension>(() => new MockMimeExt(), new MockExtensionExt()),
                new Lazy<IFileMimeType, IFileExtension>(() => new MockMimeEnt(), new MockExtensionEnt())
            };
        }

        [Test]
        public void ThrowsIfEitherFileDoesNotExist()
        {
            var manager = CreateSut();
            Assert.That(() => manager.Compare("source.ext", "exists.ext"), Throws.TypeOf<DataComparisonException>());
            Assert.That(() => manager.Compare("exists.ext", "result.ext"), Throws.TypeOf<DataComparisonException>());
        }

        [Test]
        public void ThrowsIfMimeTypesCannotBeFound()
        {
            var manager = CreateSut();
            Assert.That(() => manager.Compare("exists.unk", "exists.ext"), Throws.TypeOf<DataComparisonException>());
            Assert.That(() => manager.Compare("exists.ext", "exists.unk"), Throws.TypeOf<DataComparisonException>());
        }

        [Test]
        public void ThrowsIfMimeTypesDoNotMatch()
        {
            var manager = CreateSut();
            Assert.That(() => manager.Compare("exists.ext", "exists.ent"), Throws.TypeOf<DataComparisonException>());
        }

        [Test]
        public void CallsComparerIfExists()
        {
            var manager = CreateSut();
            var candidate = manager.Compare("exists.ext", "exists.ext");
            Assert.That(candidate, Is.EqualTo("compared"));
        }
    }
}