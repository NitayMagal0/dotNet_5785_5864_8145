namespace BO;

/// <summary>
/// Exception thrown when an entity is not found in the database.
/// </summary>
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an invalid operation is attempted on an entity.
/// </summary>
[Serializable]
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }
    public BlInvalidOperationException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an entity already exists in the database.
/// </summary>
[Serializable]
public class BlEntityAlreadyExistsException : Exception
{
    public BlEntityAlreadyExistsException(string? message) : base(message) { }
    public BlEntityAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a null reference is encountered where an object is expected.
/// </summary>
[Serializable]
public class BlNullReferenceException : Exception
{
    public BlNullReferenceException(string? message) : base(message) { }
    public BlNullReferenceException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an invalid format is encountered.
/// </summary>
[Serializable]
public class BlInvalidFormatException : Exception
{
    public BlInvalidFormatException(string? message) : base(message) { }
    public BlInvalidFormatException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }
    public BlXMLFileLoadCreateException(string message, Exception innerException) : base(message, innerException) { }
}
