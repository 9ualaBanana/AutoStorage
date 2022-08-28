using System.Timers;

namespace System.Collections.Specialized;

public class TempStorageItem<T> : IEquatable<TempStorageItem<T>>
{
    public readonly T Value;
    public GTimer? Timer;


    #region Constructors
    internal TempStorageItem(T value) : this(value, StorageTimer.Unlimited)
    {
    }

    internal TempStorageItem(KeyValuePair<T, GTimer?> pair) : this(pair.Key, pair.Value)
    {
    }

    internal TempStorageItem(T value, GTimer? timer)
    {
        Value = value;
        Timer = timer;
    }
    #endregion

    #region Equality
    public override bool Equals(object? obj) => Equals(obj as TempStorageItem<T>);
    public bool Equals(TempStorageItem<T>? other)
    {
        if (other is null) return false;
        return Value is null ? other.Value is null : Value.Equals(other.Value);
    }
    public override int GetHashCode() => Value is null ? 0 : Value.GetHashCode();
    #endregion

    #region Conversions
    public static implicit operator TempStorageItem<T>(KeyValuePair<T, GTimer?> pair) =>
        new(pair.Key, pair.Value);
    public static implicit operator KeyValuePair<T, GTimer?>(TempStorageItem<T> this_) =>
        new(this_.Value, this_.Timer);
    #endregion
}

internal static class TempStorageItemExtensions
{
    internal static IEnumerable<TempStorageItem<T>> AsTempStorageItems<T>(this IEnumerable<T> values) =>
        values.Select(value => new TempStorageItem<T>(value));

    internal static IEnumerable<T> Values<T>(this IEnumerable<TempStorageItem<T>> items) =>
        items.Select(item => item.Value);

    internal static TempStorageItem<T>? ItemWith<T>(this IEnumerable<TempStorageItem<T>> items, object elapsedStorageTimer)
    {
        try { return items.Single(item => Equals(item.Timer, elapsedStorageTimer)); }
        catch { return null; }
    }
}
