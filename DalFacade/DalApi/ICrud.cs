namespace DalApi;

public interface ICrud<T> where T : class
{
    // Create a new entity
    void Create(T item);

    // Read an entity by its ID
    T Read(int id);

    // Read all entities
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null);

    // Update an existing entity
    void Update(T item);

    // Delete an entity by its ID
    void Delete(int id);

    // Delete all entities
    void DeleteAll();
    // Read an entity by filter
    T? Read(Func<T, bool> filter);
}

