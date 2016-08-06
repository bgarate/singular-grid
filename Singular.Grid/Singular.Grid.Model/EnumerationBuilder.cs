using System.Linq;

namespace Singular.Grid.Model
{
    public static class EnumerationBuilder
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

        public interface IBuild
        {
            IBuild HasMember(string enumMember);
            Enumeration Build();
        }

        public interface IExtends : IBuild
        {
            IBuild Extends(string qualifiedName);
        }

        public class Builder : IStart, IExtends
        {
            private readonly Enumeration enumeration = new Enumeration();

            internal Builder()
            {
            }

            public IBuild Extends(string qualifiedName)
            {
                enumeration.Extends = new ModelName(qualifiedName);
                return this;
            }

            public IBuild HasMember(string enumMember)
            {
                if (!Utils.IsValidIdentifier(enumMember))
                    throw new InvalidModelException($"'{enumMember}' is not a valid member name");

                enumeration.Values.Add(enumMember);
                return this;
            }

            public Enumeration Build()
            {
                if (!enumeration.Values.Any())
                    throw new InvalidModelException($"{nameof(Enumeration)} must have at least one member");

                if (enumeration.Values.Count != enumeration.Values.Distinct().Count())
                    throw new InvalidModelException($"{nameof(Enumeration)} can't have duplicated members");

                return enumeration;
            }

            public IExtends WithName(string qualifiedName)
            {
                enumeration.Name = new ModelName(qualifiedName);
                return this;
            }
        }
    }
}