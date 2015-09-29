using System;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class DefaultConventionScanner : ConfigurableRegistrationConvention
    {
        public override void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete()) return;

            var pluginType = FindPluginType(type);
            if (pluginType != null && type.HasConstructors())
            {
                registry.AddType(pluginType, type);
                ConfigureFamily(registry.For(pluginType));
            }
        }

        public override Registry ScanTypes(TypeSet types)
        {
            var registry = new Registry();

            types.FindTypes(TypeClassification.Concretes).Where(type => type.HasConstructors()).Each(type =>
            {
                var pluginType = FindPluginType(type);
                if (pluginType != null)
                {
                    registry.AddType(pluginType, type);
                    ConfigureFamily(registry.For(pluginType));
                }
            });

            return registry;
        }

        public virtual Type FindPluginType(Type concreteType)
        {
            var interfaceName = "I" + concreteType.Name;
            return concreteType.GetInterfaces().FirstOrDefault(t => t.Name == interfaceName);
        }
    }
}