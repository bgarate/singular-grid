using System.Collections.Generic;

namespace Singular.Grid.Model
{
    public class AspectEvent
    {
        internal AspectEvent()
        {
        }

        public Dictionary<string, PropertyDefinition> Parameters { get; set; } =
            new Dictionary<string, PropertyDefinition>();
    }
}