namespace SystemTests.Context;

public interface ITestContextStorage
{
    public Dictionary<string, object?> Storage { get; }
}

/// <summary>
///     Extension methods for managing test context in-memory storage.
/// </summary>
public static class TestContextStorageExtensions
{
    public static Task Add_to_storage<T>(this ITestContextStorage context, string key, T value)
    {
        context.Storage[key] = value;
        return Task.CompletedTask;
    }

    public static T? Get_from_storage<T>(this ITestContextStorage context, string key)
    {
        return (T?)context.Storage.GetValueOrDefault(key);
    }

    public static void Clear_storage(this ITestContextStorage context) => context.Storage.Clear();
}
