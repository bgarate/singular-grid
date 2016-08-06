using System.Collections.Generic;

namespace Singular.Grid.Model
{
    public class AspectMethod
    {
        internal AspectMethod()
        {
        }

        public Dictionary<string, PropertyDefinition> Parameters { get; set; } =
            new Dictionary<string, PropertyDefinition>();

        public PropertyDefinition Returns { get; set; }
    }
}