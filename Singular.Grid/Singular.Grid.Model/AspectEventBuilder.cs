using System;

namespace Singular.Grid.Model
{
    public static class AspectEventBuilder
    {
        public interface IStart : IBuild
        {
            IStart HasParameter(string name, Func<PropertyDefinitionBuilder.IType, PropertyDefinitionBuilder.IBuild> p);
        }

        public interface IBuild
        {
        }

        public class Builder : IStart
        {
            private readonly AspectEvent aspectEvent = new AspectEvent();

            public IStart HasParameter(string name,
                Func<PropertyDefinitionBuilder.IType, PropertyDefinitionBuilder.IBuild> p)
            {
                if (!Utils.IsValidIdentifier(name))
                    throw new InvalidModelException($"'{name}' is not a valid identifier");

                if (aspectEvent.Parameters.ContainsKey(name))
                    throw new InvalidModelException($"{nameof(AspectMethod)} can't have duplicated names for parameters");

                PropertyDefinitionBuilder.Builder builder =
                    (PropertyDefinitionBuilder.Builder) p(new PropertyDefinitionBuilder.Builder());
                aspectEvent.Parameters.Add(name, builder.Build());

                return this;
            }

            public AspectEvent Build()
            {
                return aspectEvent;
            }
        }
    }
}