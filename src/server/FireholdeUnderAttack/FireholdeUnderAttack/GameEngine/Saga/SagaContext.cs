namespace FireholdeUnderAttack.GameEngine.Saga;

/// <summary>
/// Per-execution key-value bag for passing data between saga steps.
/// Created fresh for each command execution.
/// </summary>
public sealed class SagaContext
{
    private readonly Dictionary<string, object> _data = new();

    public void Set<T>(string key, T value) => _data[key] = value!;

    public T Get<T>(string key) => (T)_data[key];

    public bool TryGet<T>(string key, out T value)
    {
        if (_data.TryGetValue(key, out var obj) && obj is T typed)
        {
            value = typed;
            return true;
        }
        value = default!;
        return false;
    }
}
