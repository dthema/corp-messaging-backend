namespace Domain.Exceptions;

public class FieldNotFoundException : Exception
{
    public FieldNotFoundException()
        : base("Field not found in database") { }
    
    public FieldNotFoundException(string message)
        : base($"Field not found in database: {message}") { }
}