using System;

namespace Singular.Grid.Model
{
    public static class AspectMethodBuilder
    {
        public interface IStart : IReturns
        {
            IStart HasParameter(string name, Func<PropertyDefinitionBuilder.IType, PropertyDefinitionBuilder.IBuild> p);
        }

        public interface IReturns : IBuild
        {
            IBuild Returns(Func<PropertyDefinitionBuilder.IType, PropertyDefinitionBuilder.IBuild> r);
        }

        public interface IBuild
        {
        }

        public class Builder : IStart
        {
            private readonly AspectMethod method = new AspectMethod();

            public IBuild Returns(Func<PropertyDefinitionBuilder.IType, PropertyDefinitionBuilder.IBuild> r)
            {
                PropertyDefinitionBuilder.Builder builder =
                    (PropertyDefinitionBuilder.Builder) r(new PropertyDefinitionBuilder.Builder());
                method.Returns = builder.Build();
                return this;
            }

            public IStart HasParameter(string name,
                Func<PropertyDefinitionBuilder.IType, PropertyDefinitionBuilder.IBuild> p)
            {
                if (!Utils.IsValidIdentifier(name))
                    throw new InvalidModelException($"'{name}' is not a valid identifier");

                if (method.Parameters.ContainsKey(name))
                    throw new InvalidModelException($"{nameof(AspectMethod)} can't have duplicated names for parameters");

                PropertyDefinitionBuilder.Builder builder =
                    (PropertyDefinitionBuilder.Builder) p(new PropertyDefinitionBuilder.Builder());
                method.Parameters.Add(name, builder.Build());


                return this;
            }

            internal AspectMethod Build()
            {
                return method;
            }
        }
    }
}