namespace Singular.Grid.Model
{
    public class PropertyDefinition
    {
        internal PropertyDefinition()
        {
        }

        public Presence Presence { get; set; }
        public Multiplicity Multiplicity { get; set; }
        public PropertyTypes Type { get; set; }
        public ModelName TypeReferences { get; set; }
        public PropertyModifier Modifier { get; set; }
    }

    public enum PropertyModifier
    {
        ReadWrite,
        ReadOnly,
        WriteOnly
    }
}