using System.Diagnostics.CodeAnalysis;
using System.Timers;

namespace System.Collections.Specialized;

public class StorageTimer
{
    private readonly GTimer? _timer;

    public static readonly StorageTimer Unlimited = new(StorageTime.Unlimited);

    [MemberNotNullWhen(false, nameof(_timer))]
    public bool IsUnlimited => _timer == (GTimer?)Unlimited;

    #region WrappedGTimerInterface
    bool AutoReset { set { if (!IsUnlimited) _timer.AutoReset = value; } }

    public DateTimeOffset CreationTime;

    public DateTimeOffset LastResetTime => _timer?.LastResetTime ?? CreationTime;

    internal event ElapsedEventHandler Elapsed
    {
        add { if (_timer is not null) { _timer.Elapsed += value; _elapsed += value; } }
        remove { if (_timer is not null) { _timer.Elapsed -= value; _elapsed += value; } }
    }
    event ElapsedEventHandler? _elapsed;

    public StorageTime Interval => _timer?.Interval ?? StorageTime.Unlimited;

    void Start() => _timer?.Start();

    internal void Restart() => _timer?.Restart();
    #endregion


    #region Constructors
    StorageTimer(StorageTime storageTime)
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


    internal StorageTimer Initialize(params ElapsedEventHandler[] with)
    {
        if (!with.Any()) throw new MissingMethodException(
            $"Storage timer must be initialized with {nameof(ElapsedEventHandler)} responsible for removing items with elapsed storage time.");

        foreach (var elapsedEventHandler in with)
            Elapsed += elapsedEventHandler;
        AutoReset = false;
        Start();
        return this;
    } 


    #region Conversions
    public static implicit operator StorageTimer(GTimer? timer) => new(timer);
    public static implicit operator GTimer?(StorageTimer storageTimer) => storageTimer._timer;
    #endregion
}
