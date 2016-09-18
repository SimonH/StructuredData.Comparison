using System.ComponentModel.Composition.Hosting;

namespace StructuredData.Comparison.Container
{
    internal static class ContainerManager
    {
        public static void SetExplicitContainer(CompositionContainer container)
        {
            _compositionContainer = container;
        }

        private static CompositionContainer _compositionContainer;
        private static CompositionContainer BuildContainer()
        {
            return new CompositionContainer(new ApplicationCatalog());
        } 

        public static CompositionContainer CompositionContainer => _compositionContainer ?? (_compositionContainer = BuildContainer());
    }
}