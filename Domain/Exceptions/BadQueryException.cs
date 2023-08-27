namespace Domain.Exceptions;

public class BadQueryException : Exception
{
    public BadQueryException()
        : base("Could not execute query") { }
    
    public BadQueryException(string message)
        : base($"Could not execute query: {message}") { }
}