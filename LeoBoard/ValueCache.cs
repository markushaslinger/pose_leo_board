using Key = (int Row, int Column);

namespace LeoBoard;

public sealed class ValueCache
{
    private readonly Dictionary<Key, string> _cache = new();
    
    public string this[int row, int column]
    {
        get => _cache.GetValueOrDefault((row, column)) ?? string.Empty;
        set => _cache[(row, column)] = value;
    }

    public void Clear()
    {
        _cache.Clear();
    }
}