using System.Timers;

namespace System.Collections.Specialized;

public class TempStorageItem<T> : IEquatable<TempStorageItem<T>>
{
    public readonly T Value;
    public readonly GTimer Timer;


    public TempStorageItem(T value, GTimer timer)
    {
        Value = value;
        Timer = timer;
    }

    public TempStorageItem(KeyValuePair<T, GTimer> pair) : this(pair.Key, pair.Value)
    {
    }


    public override bool Equals(object? obj) => Equals(obj as TempStorageItem<T>);
    public bool Equals(TempStorageItem<T>? other)
    {
        if (other is null) return false;
        return Value is null ? other.Value is null : Value.Equals(other.Value);
    }
    public override int GetHashCode() => Value is null ? 0 : Value.GetHashCode();


    public static implicit operator TempStorageItem<T>(KeyValuePair<T, GTimer> pair) =>
        new(pair.Key, pair.Value);
    public static implicit operator KeyValuePair<T, GTimer>(TempStorageItem<T> this_) =>
        new(this_.Value, this_.Timer);
}
