using System.Collections.Generic;

namespace Singular.Grid.Model
{
    public class Enumeration : IDataType
    {
        public ModelName Extends { get; set; }
        public List<string> Values { get; set; } = new List<string>();
        public ModelName Name { get; set; }
    }
}