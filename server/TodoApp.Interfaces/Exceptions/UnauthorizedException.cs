namespace TodoApp.Interfaces.Exceptions;

/// <summary>Wrong credentials. Mapped to HTTP 401.</summary>
public class UnauthorizedException(string message) : Exception(message);
