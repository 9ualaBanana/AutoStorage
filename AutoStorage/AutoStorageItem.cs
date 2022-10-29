//MIT License

//Copyright (c) 2022 9ualaBanana

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

namespace System.Collections.Specialized;

/// <summary>
/// Internal representation of values stored inside <see cref="AutoStorage{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class AutoStorageItem<T> : IEquatable<AutoStorageItem<T>>
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public T Value { get; internal set; }
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

internal static class AutoStorageItemExtensions
{
    internal static IEnumerable<AutoStorageItem<T>> AsAutoStorageItems<T>(this IEnumerable<T> values) =>
        values.Select(value => new AutoStorageItem<T>(value));

    internal static IEnumerable<T> Values<T>(this IEnumerable<AutoStorageItem<T>> items) =>
        items.Select(item => item.Value);

    internal static AutoStorageItem<T>? ItemWith<T>(this IEnumerable<AutoStorageItem<T>> items, object elapsedStorageTimer)
    {
        try { return items.Single(item => Equals(item.Timer, elapsedStorageTimer)); }
        catch { return null; }
    }
}
