namespace TodoApp.Interfaces.Exceptions;

/// <summary>Request conflicts with existing data (e.g. duplicate email). Mapped to HTTP 409.</summary>
public class ConflictException(string message) : Exception(message);
