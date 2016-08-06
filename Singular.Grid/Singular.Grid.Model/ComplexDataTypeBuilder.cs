using System;
using System.Linq;

namespace Singular.Grid.Model
{
    public static class ComplexDataTypeBuilder
    {
        public static IExtends Define(string qualifiedName)
        {
            Builder builder = new Builder();
            builder.WithName(qualifiedName);
            return builder;
        }

        public interface IStart
        {
            IExtends WithName(string qualifiedName);
        }

        public interface IExtends : IBuild
        {
            IBuild Extends(string qualifiedName);
        }

        public interface IBuild
        {
            IBuild Has(string name, Func<PropertyDefinitionBuilder.IStart, PropertyDefinitionBuilder.IBuild> property);
            ComplexDataType Build();
        }

        public class Builder : IStart, IExtends
        {
            private readonly ComplexDataType dataType = new ComplexDataType();

            internal Builder()
            {
            }

            public IBuild Extends(string qualifiedName)
            {
                dataType.Extends = new ModelName(qualifiedName);
                return this;
            }

            public IBuild Has(string name,
                Func<PropertyDefinitionBuilder.IStart, PropertyDefinitionBuilder.IBuild> property)
            {
                if (!Utils.IsValidIdentifier(name))
                    throw new InvalidModelException($"'{name}' is not a valid property identifier");

                PropertyDefinitionBuilder.Builder builder =
                    (PropertyDefinitionBuilder.Builder) property(new PropertyDefinitionBuilder.Builder());

                PropertyDefinition propertyDefinition = builder.Build();

                if (dataType.Properties.ContainsKey(name))
                    throw new InvalidModelException(
                        $"{nameof(ComplexDataType)} can't have duplicated names for properties");

                dataType.Properties.Add(name, propertyDefinition);
                return this;
            }

            public ComplexDataType Build()
            {
                if (!dataType.Properties.Any())
                    throw new InvalidModelException($"{nameof(ComplexDataType)} must have at leas one property defined");

                return dataType;
            }

            public IExtends WithName(string qualifiedName)
            {
                dataType.Name = new ModelName(qualifiedName);
                return this;
            }
        }
    }
}