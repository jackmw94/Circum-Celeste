using System;

public class UnexpectedValuesException : Exception
{
    public UnexpectedValuesException(string message) : base(message){}
}