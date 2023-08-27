namespace Domain.Exceptions;

public class EmptyStringBadQueryException : Exception
{
    private EmptyStringBadQueryException(string message)
        : base($"Could not execute query: {message}") { }

    public static void ThrowIfStringNull(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new EmptyStringBadQueryException(str);
    }
}