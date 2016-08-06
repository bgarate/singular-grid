using System.Text.RegularExpressions;

namespace Singular.Grid.Model
{
    public static class Utils
    {
        public static bool IsValidIdentifier(string identifier)
        {
            Regex regex = new Regex(@"^[^\W\d]\w*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

            return identifier == null || regex.IsMatch(identifier);
        }


        public static bool IsValidModelName(string qualifiedName)
        {
            return ModelName.IsValid(qualifiedName);
        }
    }
}