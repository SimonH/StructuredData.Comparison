# StructuredData.Comparison
Extensible core library for comparing structured transport data (e.g. xml json).

##Usage
 StructuredData.Comparison is not used in isolation. 
 
 To use install one or more implementation library (e.g. StructuredData.Comparison.Json) and then use one of the StructureDataComparison extension methods.  
 [e.g. "SourceFilePath.json".FileComparison("ResultFilePath.json")]
 
 When comparing using only file paths the library 
 * Uses [Media Type Map](https://www.nuget.org/packages/MediaTypeMap/) to determine the mimetype of each file
 * Makes sure that both files are the same mimetype
 * Tries _using MEF_ to locate an implemenation of ICreateStructuredDataWalkers for the mimeType in any loaded application library
 * Performs the comparison
 
When comparing text data _using ContentComparison you supply the mimeType  
[e.g. "SourceData".ContentComparison("ResultData", "application/json")]

The result data can contain embedded settings or inline value processors to directly influence the comparison

{
  "StructuredData.Comparison_Settings":{
     "StringComparison":"Ordinal" - Allows Case and Culture Comparison based on the .Net StringComparison enum
  }
  "StaticProperty":"actual value",
  "CurrentDateTime":"StructuredData.Comparison_Ignore" - tells the comparison to ignore this value which changes for different results
}
 

##Extending the library
 You can create your own comparison libraries by implementing ICreateStructuredDataWalkers and exporting it with the correct mimeType
  
   [Export(typeof(ICreateStructuredDataWalkers))]  
   [ExportMetadata("MimeType", "application/json")]  
   public class WalkerCreator : ICreateStructuredDataWalkers
   
New Value Processors can be created by exporting an implementation of IValueProcessorCommand (currently they are expected to be found as "StructuredData.Comparison_<name>" but **BEWARE!** this may change in the future as the functionality is enhanced)

[E.g. The Ignore command above is exported as 

   [Export(typeof(IValueProcessorCommand))]  
   [ExportMetadata("Name", "Ignore")]  
   public class IgnoreValueProcessorCommand : IValueProcessorCommand
