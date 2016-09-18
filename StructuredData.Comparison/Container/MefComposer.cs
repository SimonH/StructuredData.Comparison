using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

namespace StructuredData.Comparison.Container
{
    public static class MefComposer
    {
        public static void SatisfyImports<T>(ComposablePart partInstance)
        {
            ContainerManager.CompositionContainer.SatisfyImportsOnce(partInstance);
        }
    }
}