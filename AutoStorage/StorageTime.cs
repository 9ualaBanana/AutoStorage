using System.Timers;

namespace System.Collections.Specialized;

public struct StorageTime
{
    readonly Interval? _value;

    public static readonly Interval? Unlimited = null;
    public static readonly Interval Default = default;

    public bool IsUnlimited => _value == Unlimited;
    public bool IsDefault => _value == Default;


    StorageTime(Interval? storageTime) => _value = storageTime;


    internal static StorageTime Of(StorageTimer storageTimer) => storageTimer.IsUnlimited ?
        StorageTime.Unlimited : storageTimer.Interval;

    #region Conversions
    public static implicit operator StorageTime(Interval? storageTime) => new(storageTime);
    public static implicit operator Interval?(StorageTime storageTime) => storageTime._value;

    public static implicit operator StorageTime(TimeSpan? storageTime) => new(storageTime);
    public static implicit operator TimeSpan?(StorageTime storageTime) => storageTime._value;

    public static implicit operator StorageTime(double? storageTime) => new(storageTime);
    public static implicit operator double?(StorageTime storageTime) => storageTime._value;
    #endregion
}
