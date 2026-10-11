namespace BoardingHouse.Application.Common.Exceptions;

public sealed class ValidationException(string message) : Exception(message);
