using System.Timers;

namespace System.Collections.Specialized;

public class AutoStorageItem<T> : IEquatable<AutoStorageItem<T>>
{
    public readonly T Value;
    public StorageTimer Timer;


    #region Constructors
    internal AutoStorageItem(T value) : this(value, StorageTimer.Unlimited)
    {
    }

    internal AutoStorageItem(KeyValuePair<T, StorageTimer> pair) : this(pair.Key, pair.Value)
    {
    }

    internal AutoStorageItem(T value, StorageTimer timer)
    {
        Value = value;
        Timer = timer;
    }
    #endregion


    #region Equality
    public override bool Equals(object? obj) => Equals(obj as AutoStorageItem<T>);
    public bool Equals(AutoStorageItem<T>? other)
    {
        if (other is null) return false;
        return Value is null ? other.Value is null : Value.Equals(other.Value);
    }
    public override int GetHashCode() => Value is null ? 0 : Value.GetHashCode();
    #endregion

    #region Conversions
    public static implicit operator AutoStorageItem<T>(KeyValuePair<T, StorageTimer> pair) =>
        new(pair.Key, pair.Value);
    public static implicit operator KeyValuePair<T, StorageTimer>(AutoStorageItem<T> this_) =>
        new(this_.Value, this_.Timer);
    #endregion
}

internal static class TempStorageItemExtensions
{
    internal static IEnumerable<AutoStorageItem<T>> AsTempStorageItems<T>(this IEnumerable<T> values) =>
        values.Select(value => new AutoStorageItem<T>(value));

    internal static IEnumerable<T> Values<T>(this IEnumerable<AutoStorageItem<T>> items) =>
        items.Select(item => item.Value);

    internal static AutoStorageItem<T>? ItemWith<T>(this IEnumerable<AutoStorageItem<T>> items, object elapsedStorageTimer)
    {
        try { return items.Single(item => Equals(item.Timer, elapsedStorageTimer)); }
        catch { return null; }
    }
}
