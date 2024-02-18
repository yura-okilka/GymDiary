namespace SystemTests;

/// <summary>
///     Context for executing test steps and storing step results.
/// </summary>
public abstract class TestContextBase
{
    private readonly Dictionary<string, object?> _storage = new();

    public Task Add_to_storage<T>(string key, T value)
    {
        _storage[key] = value;
        return Task.CompletedTask;
    }

    public T? Get_from_storage<T>(string key) => (T?)_storage.GetValueOrDefault(key);

    public void Clear_storage() => _storage.Clear();
}
