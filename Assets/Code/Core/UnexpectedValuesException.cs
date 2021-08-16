using System;

namespace Code.Core
{
    public class UnexpectedValuesException : Exception
    {
        public UnexpectedValuesException(string message) : base(message){}
    }
}