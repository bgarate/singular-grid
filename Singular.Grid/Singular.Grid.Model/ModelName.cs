using System.Linq;
using System.Text.RegularExpressions;

namespace Singular.Grid.Model
{
    public class ModelName
    {
        private string fullyQualifiedName;

        public ModelName(string qualifiedName)
        {
            FullyQualifiedName = qualifiedName;
        }

        public string FullyQualifiedName
        {
            get { return fullyQualifiedName; }
            set
            {
                if (!IsValid(value))
                    throw new InvalidModelException($"'{value}' is not a valid value for {nameof(FullyQualifiedName)}");

                fullyQualifiedName = value;
            }
        }

        public string Name => FullyQualifiedName.Split('.').Last();

        public string Namespace
        {
            get
            {
                int lastSeparator = FullyQualifiedName.LastIndexOf('.');
                return lastSeparator == -1 ? string.Empty : FullyQualifiedName.Substring(0, lastSeparator);
            }
        }

        public static bool IsValid(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            Regex regex = new Regex(@"^([^\d\W]\w*\.)*[^\d\W]\w*$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);
            return regex.IsMatch(str);
        }

        public override string ToString()
        {
            return FullyQualifiedName;
        }
    }
}