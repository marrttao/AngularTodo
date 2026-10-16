namespace TodoApp.Interfaces.Exceptions;

/// <summary>Requested resource doesn't exist (or belongs to another user). Mapped to HTTP 404.</summary>
public class NotFoundException(string message) : Exception(message);
