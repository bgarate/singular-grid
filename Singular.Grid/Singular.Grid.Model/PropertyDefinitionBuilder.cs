namespace Singular.Grid.Model
{
    public static class PropertyDefinitionBuilder
    {
        public interface IOptional : IMultiple
        {
            IMultiple Optional();
        }

        public interface IMultiple : IBuild
        {
            IBuild Multiple();
        }

        public interface IType
        {
            IOptional FromPrimitiveType(PropertyTypes type);
            IReferences FromEnum();
            IReferences FromDataType();
        }

        public interface IReferences
        {
            IOptional References(string qualifiedName);
        }

        public interface IBuild
        {
        }

        public interface IStart : IType
        {
            IType ReadOnly();
            IType WriteOnly();
        }

        internal class Builder : IReferences, IStart, IOptional
        {
            private readonly PropertyDefinition property = new PropertyDefinition();

            public IMultiple Optional()
            {
                property.Presence = Presence.Optional;
                return this;
            }

            public IBuild Multiple()
            {
                property.Multiplicity = Multiplicity.Multiple;
                return this;
            }

            public IOptional References(string qualifiedName)
            {
                property.TypeReferences = new ModelName(qualifiedName);
                return this;
            }

            public IOptional FromPrimitiveType(PropertyTypes type)
            {
                if (type == PropertyTypes.Enum)
                    throw new InvalidModelException(
                        $"To define a property of type '{type}' use the method '{nameof(FromEnum)}'");

                if (type == PropertyTypes.DataType)
                    throw new InvalidModelException(
                        $"To define a property of type '{type}' use the method '{nameof(FromDataType)}'");

                property.Type = type;
                return this;
            }

            public IReferences FromEnum()
            {
                property.Type = PropertyTypes.Enum;
                return this;
            }

            public IReferences FromDataType()
            {
                property.Type = PropertyTypes.DataType;
                return this;
            }

            public IType ReadOnly()
            {
                property.Modifier = PropertyModifier.ReadOnly;
                return this;
            }

            public IType WriteOnly()
            {
                property.Modifier = PropertyModifier.WriteOnly;
                return this;
            }

            public PropertyDefinition Build()
            {
                return property;
            }
        }
    }
}