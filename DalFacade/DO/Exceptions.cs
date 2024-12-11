namespace DO;


/// <summary>
/// Exception thrown when an entity is not found in the database.
/// </summary>
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an invalid operation is attempted on an entity.
/// </summary>
[Serializable]
public class DalInvalidOperationException : Exception
{
    public DalInvalidOperationException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an entity already exists in the database.
/// </summary>
[Serializable]
public class DalEntityAlreadyExistsException : Exception
{
    public DalEntityAlreadyExistsException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when a null reference is encountered where an object is expected.
/// </summary>
[Serializable]
public class DalNullReferenceException : Exception
{
    public DalNullReferenceException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an invalid format is encountered.
/// </summary>
[Serializable]
public class DalInvalidFormatException : Exception
{
    public DalInvalidFormatException(string? message) : base(message) { }
}
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}



