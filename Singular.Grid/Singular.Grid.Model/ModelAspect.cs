using System;
using System.Collections.Generic;
using System.Linq;

namespace Singular.Grid.Model
{
    public class ModelAspect : IAdressable
    {
        internal ModelAspect()
        {
        }

        public Dictionary<string, PropertyDefinition> Properties { get; set; } =
            new Dictionary<string, PropertyDefinition>();

        public Dictionary<string, AspectMethod> Methods { get; set; } = new Dictionary<string, AspectMethod>();
        public Dictionary<string, AspectEvent> Events { get; set; } = new Dictionary<string, AspectEvent>();
        public ModelName Name { get; set; }
    }

    public static class ModelAspectBuilder
    {
        public static IAspect Define(string qualifiedName)
        {
            Builder builder = new Builder();
            builder.WithName(qualifiedName);
            return builder;
        }

        public interface IStart
        {
            IAspect WithName(string qualifiedName);
        }

        public interface IAspect
        {
            IAspect Property(string name,
                Func<PropertyDefinitionBuilder.IStart, PropertyDefinitionBuilder.IBuild> p);

            IAspect Methods(string name,
                Func<AspectMethodBuilder.IStart, AspectMethodBuilder.IBuild> m);

            IAspect Events(string name,
                Func<AspectEventBuilder.IStart, AspectEventBuilder.IBuild> e);

            ModelAspect Build();
        }

        public class Builder : IStart, IAspect
        {
            private readonly ModelAspect model = new ModelAspect();

            public IAspect Property(string name,
                Func<PropertyDefinitionBuilder.IStart, PropertyDefinitionBuilder.IBuild> p)
            {
                if (!Utils.IsValidIdentifier(name))
                    throw new InvalidModelException($"'{name}' is not a valid property identifier");

                PropertyDefinitionBuilder.Builder builder =
                    (PropertyDefinitionBuilder.Builder) p(new PropertyDefinitionBuilder.Builder());

                PropertyDefinition propertyDefinition = builder.Build();

                if (HasMember(name))
                    throw new InvalidModelException($"{nameof(ComplexDataType)} can't have duplicated names for members");

                model.Properties.Add(name, propertyDefinition);
                return this;
            }

            public IAspect Methods(string name, Func<AspectMethodBuilder.IStart, AspectMethodBuilder.IBuild> m)
            {
                if (!Utils.IsValidIdentifier(name))
                    throw new InvalidModelException($"'{name}' is not a valid property identifier");

                AspectMethodBuilder.Builder builder =
                    (AspectMethodBuilder.Builder) m(new AspectMethodBuilder.Builder());

                AspectMethod methodDefinition = builder.Build();

                if (HasMember(name))
                    throw new InvalidModelException($"{nameof(ComplexDataType)} can't have duplicated names for members");

                model.Methods.Add(name, methodDefinition);
                return this;
            }

            public IAspect Events(string name, Func<AspectEventBuilder.IStart, AspectEventBuilder.IBuild> e)
            {
                if (!Utils.IsValidIdentifier(name))
                    throw new InvalidModelException($"'{name}' is not a valid property identifier");

                AspectEventBuilder.Builder builder =
                    (AspectEventBuilder.Builder) e(new AspectEventBuilder.Builder());

                AspectEvent eventDefinition = builder.Build();

                if (HasMember(name))
                    throw new InvalidModelException($"{nameof(ComplexDataType)} can't have duplicated names for members");

                model.Events.Add(name, eventDefinition);
                return this;
            }

            public ModelAspect Build()
            {
                if (!model.Properties.Any() && !model.Methods.Any() && !model.Events.Any())
                    throw new InvalidModelException($"{nameof(ModelAspect)} must have at least one member defined");

                return model;
            }

            public IAspect WithName(string qualifiedName)
            {
                model.Name = new ModelName(qualifiedName);
                return this;
            }

            private bool HasMember(string name)
            {
                return model.Properties.ContainsKey(name) || model.Events.ContainsKey(name) ||
                       model.Methods.ContainsKey(name);
            }
        }
    }
}