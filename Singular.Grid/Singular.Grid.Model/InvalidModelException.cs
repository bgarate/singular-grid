using System;

namespace Singular.Grid.Model
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException(string s) : base(s)
        {
        }
    }
}