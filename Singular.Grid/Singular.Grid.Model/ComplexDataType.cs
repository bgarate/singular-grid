using System.Collections.Generic;

namespace Singular.Grid.Model
{
    public class ComplexDataType : IDataType
    {
        internal ComplexDataType()
        {
        }

        public ModelName Extends { get; set; }
        public string Description { get; set; }

        public Dictionary<string, PropertyDefinition> Properties { get; set; } =
            new Dictionary<string, PropertyDefinition>();

        public ModelName Name { get; set; }
    }
}