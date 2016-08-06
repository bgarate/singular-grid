using System;

namespace Singular.Grid.Core.Exceptions
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException(string s) : base(s)
        {
        }
    }
}