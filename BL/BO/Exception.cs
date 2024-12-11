namespace BO;

/// <summary>
/// Exception thrown when an entity is not found in the database.
/// </summary>
[Serializable]
public class BoDoesNotExistException : Exception
{
    public BoDoesNotExistException(string? message) : base(message) { }
    public BoDoesNotExistException(string? message, Exception? innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an invalid operation is attempted on an entity.
/// </summary>
[Serializable]
public class BoInvalidOperationException : Exception
{
    public BoInvalidOperationException(string? message) : base(message) { }
    public BoInvalidOperationException(string? message, Exception? innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an entity already exists in the database.
/// </summary>
[Serializable]
public class BoEntityAlreadyExistsException : Exception
{
    public BoEntityAlreadyExistsException(string? message) : base(message) { }
    public BoEntityAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a null reference is encountered where an object is expected.
/// </summary>
[Serializable]
public class BoNullReferenceException : Exception
{
    public BoNullReferenceException(string? message) : base(message) { }
    public BoNullReferenceException(string? message, Exception? innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an invalid format is encountered.
/// </summary>
[Serializable]
public class BoInvalidFormatException : Exception
{
    public BoInvalidFormatException(string? message) : base(message) { }
    public BoInvalidFormatException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BoXMLFileLoadCreateException : Exception
{
    public BoXMLFileLoadCreateException(string? message) : base(message) { }
    public BoXMLFileLoadCreateException(string? message, Exception? innerException) : base(message, innerException) { }
}
