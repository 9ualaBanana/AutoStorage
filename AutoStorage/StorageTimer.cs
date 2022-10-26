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
