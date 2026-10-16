namespace TodoApp.Interfaces.Exceptions;

/// <summary>Request is invalid from the business logic point of view. Mapped to HTTP 400.</summary>
public class BadRequestException(string message) : Exception(message);
