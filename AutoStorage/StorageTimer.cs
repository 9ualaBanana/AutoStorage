﻿using System.Diagnostics.CodeAnalysis;
using System.Timers;

namespace System.Collections.Specialized;

public class StorageTimer
{
    private readonly GTimer? _timer;

    public static readonly StorageTimer Unlimited = new(StorageTime.Unlimited);

    [MemberNotNullWhen(false, nameof(_timer))]
    public bool IsUnlimited => _timer == Unlimited._timer;

    internal bool IsInitialized { get; private set; }

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

    internal bool Enabled => _timer?.Enabled ?? false;

    public StorageTime Interval => _timer?.Interval ?? StorageTime.Unlimited;

    void Start() => _timer?.Start();

    internal void Restart() => _timer?.Restart();

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
