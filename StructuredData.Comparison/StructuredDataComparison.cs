using System;
using System.IO;
using StructuredData.Comparison.Container;
using StructuredData.Comparison.Interfaces;

namespace StructuredData.Comparison
{
    public static class StructuredDataComparison
    {
        private static IManageStructuredDataComparison GetComparisonManager()
        {
            return ContainerManager.CompositionContainer.GetExport<IManageStructuredDataComparison>()?.Value;
        }

        public static string Comparison(this FileInfo sourceFile, FileInfo resultDeclarationFile)
        {
            return FileComparison(sourceFile.FullName, resultDeclarationFile.FullName);
        }

        public static bool IsEqualTo(this FileInfo sourceFile, FileInfo resultDeclarationFile)
        {
            return CheckEquality(() => sourceFile.Comparison(resultDeclarationFile));
        }

        public static string FileComparison(this string sourceFilePath, string resultDeclarationFilePath)
        {
            return GetComparisonManager().Compare(sourceFilePath, resultDeclarationFilePath);
        }

        public static bool IsEqualTo(this string sourceFilePath, string resultDeclarationFilePath)
        {
            return CheckEquality(() => sourceFilePath.FileComparison(resultDeclarationFilePath));
        }

        public static string ContentComparison(this string sourceData, string resultDeclarationData, string mimeType)
        {
            return GetComparisonManager().Compare(sourceData, resultDeclarationData, mimeType);
        }

        public static bool IsEqualTo(this string sourceData, string resultDeclarationData, string mimeType)
        {
            return CheckEquality(() => ContentComparison(sourceData, resultDeclarationData, mimeType));
        }

        private static bool CheckEquality(Func<string> func)
        {
            return string.IsNullOrWhiteSpace(func());
        }
    }
}