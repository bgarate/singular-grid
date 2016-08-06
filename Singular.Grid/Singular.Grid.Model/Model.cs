using System.Collections.Generic;
using System.Linq;

namespace Singular.Grid.Model
{
    public class Model : IAdressable
    {
        internal Model()
        {
        }

        public ModelName Extends { get; set; }
        public List<ModelName> Aspects { get; set; } = new List<ModelName>();
        public ModelName Name { get; set; }
    }

    public static class ModelBuilder
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

        public interface IExtends : IAspect
        {
            IAspect Extends(string qualifiedName);
        }

        public interface IAspect
        {
            IAspect Aspect(string qualifiedName);
            Model Build();
        }

        public class Builder : IStart, IExtends
        {
            private readonly Model model = new Model();

            public IAspect Aspect(string qualifiedName)
            {
                model.Aspects.Add(new ModelName(qualifiedName));
                return this;
            }

            public Model Build()
            {
                if (!model.Aspects.Any())
                    throw new InvalidModelException($"{nameof(Model)} must implement at least one aspect");

                if (model.Aspects.Count != model.Aspects.Distinct().Count())
                    throw new InvalidModelException($"{nameof(Model)} cannot implement an aspect more than once");

                return model;
            }

            public IAspect Extends(string qualifiedName)
            {
                model.Extends = new ModelName(qualifiedName);
                return this;
            }

            public IExtends WithName(string qualifiedName)
            {
                model.Name = new ModelName(qualifiedName);
                return this;
            }
        }
    }
}