using System.Diagnostics.CodeAnalysis;
using System.Timers;

namespace System.Collections.Specialized;

/// <summary>
/// Used for supervising the storage time of values inside <see cref="AutoStorage{T}"/>.
/// </summary>
public class StorageTimer
{
    private readonly GTimer? _timer;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static readonly StorageTimer Unlimited = new(StorageTime.Unlimited);

    [MemberNotNullWhen(false, nameof(_timer))]
    public bool IsUnlimited => _timer == Unlimited._timer;

    internal bool IsInitialized { get; private set; }

    #region WrappedGTimerInterface
    bool AutoReset { set { if (!IsUnlimited) _timer.AutoReset = value; } }

    public DateTimeOffset CreationTime;

    public DateTimeOffset LastResetTime => _timer?.LastResetTime ?? CreationTime;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    internal event ElapsedEventHandler Elapsed
    {
        add { if (_timer is not null) { _timer.Elapsed += value; _elapsed += value; } }
        remove { if (_timer is not null) { _timer.Elapsed -= value; _elapsed += value; } }
    }
    event ElapsedEventHandler? _elapsed;

    internal bool Enabled => _timer?.Enabled ?? false;

    /// <summary>
    /// Storage time after which the corresponding value is removed from <see cref="AutoStorage{T}"/> unless reset.
    /// </summary>
    public StorageTime Interval => _timer?.Interval ?? StorageTime.Unlimited;

    void Start() => _timer?.Start();

    internal void Restart() => _timer?.Restart();

    /// <summary>
    /// Time spent in <see cref="Enabled"/> state.
    /// </summary>
    public Interval Uptime => _timer?.Uptime ?? TimeSpan.Zero;
    #endregion


    #region Constructors
    internal StorageTimer(StorageTime storageTime)
        : this(storageTime.IsUnlimited ? null : new GTimer((Interval)storageTime!))
    {
    }

    StorageTimer(GTimer? timer)
    {
        _timer = timer;
        CreationTime = _timer?.CreationTime ?? DateTimeOffset.Now;

        void spoofElapsedEvent(object? _, ElapsedEventArgs e) => _elapsed?.Invoke(this, e);
        if (_timer is not null) _timer.Elapsed += spoofElapsedEvent;
    }
    #endregion


    internal StorageTimer InitializeWith(ElapsedEventHandler onElapsed)
    {
        Elapsed += onElapsed;
        AutoReset = false;
        Start();
        IsInitialized = true;
        return this;
    } 
}
