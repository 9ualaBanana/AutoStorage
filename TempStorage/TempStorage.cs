using System.Timers;

namespace System.Collections.Specialized;

public class TempStorage<T> : HashSet<TempStorageItem<T>>
{
    public readonly TimeSpan DefaultStorageTime;



    public TempStorage() : this(EqualityComparer<TempStorageItem<T>>.Default, TimeSpan.Zero)
    {
    }

    public TempStorage(double defaultStorageTime)
        : this(EqualityComparer<TempStorageItem<T>>.Default, TimeSpan.FromMilliseconds(defaultStorageTime))
    {
    }

    public TempStorage(TimeSpan defaultStorageTime)
        : this(EqualityComparer<TempStorageItem<T>>.Default, defaultStorageTime)
    {
    }

    public TempStorage(IEqualityComparer<TempStorageItem<T>>? comparer, double defaultStorageTime = default)
        : this(comparer, TimeSpan.FromMilliseconds(defaultStorageTime))
    {
    }

    public TempStorage(IEqualityComparer<TempStorageItem<T>>? comparer, TimeSpan defaultStorageTime = default)
        : base(comparer)
    {
        DefaultStorageTime = defaultStorageTime;
    }



    public TempStorage(int capacity, double defaultStorageTime = default)
        : this(capacity, null, TimeSpan.FromMilliseconds(defaultStorageTime))
    {
    }

    public TempStorage(int capacity, TimeSpan defaultStorageTime = default)
        : this(capacity, null, defaultStorageTime)
    {
    }

    public TempStorage(int capacity, IEqualityComparer<TempStorageItem<T>>? comparer, double defaultStorageTime = default)
        : this(capacity, comparer, TimeSpan.FromMilliseconds(defaultStorageTime))
    {
    }

    public TempStorage(int capacity, IEqualityComparer<TempStorageItem<T>>? comparer, TimeSpan defaultStorageTime = default)
        : base(capacity, comparer)
    {
        DefaultStorageTime = defaultStorageTime;
    }



    public TempStorage(IEnumerable<T> collection, double defaultStorageTime = default)
        : this(collection, TimeSpan.FromMilliseconds(defaultStorageTime))
    {
    }

    public TempStorage(IEnumerable<T> collection, TimeSpan defaultStorageTime = default)
        : this(collection, null, defaultStorageTime)
    {
    }

    public TempStorage(IEnumerable<T> collection, IEqualityComparer<TempStorageItem<T>>? comparer, double defaultStorageTime = default)
        : this(collection, comparer, TimeSpan.FromMilliseconds(defaultStorageTime))
    {
    }

    public TempStorage(IEnumerable<T> collection, IEqualityComparer<TempStorageItem<T>>? comparer, TimeSpan defaultStorageTime = default)
        : base(collection.Select(element => new TempStorageItem<T>(element, new GTimer(defaultStorageTime))), comparer)
    {
        DefaultStorageTime = defaultStorageTime;
        foreach (var tempStorageItem in this)
            InitializeAndStartStorageTimer(tempStorageItem.Timer);
    }



    /// <summary>
    /// Initializes and starts the <paramref name="timer"/>.
    /// </summary>
    /// <param name="timer">The timer to initialize <see cref="TempStorageItem{T}"/> with.</param>
    /// <param name="storageTime">The storage time to initialize <see cref="TempStorageItem{T}.Timer"/> with;
    /// if <i>null</i>, it's assumed that timer's storage time is already set.</param>
    /// <returns>Started initialized <paramref name="timer"/>.</returns>
    GTimer InitializeAndStartStorageTimer(GTimer timer, double? storageTime = null)
    {
        if (storageTime is not null) 
            timer.Interval = storageTime.Value;
        timer.Elapsed += OnStorageTimeElapsed;
        if (!StorageTimeIsUnlimited(timer.Interval))
            timer.Start();
        return timer;
    }

    void OnStorageTimeElapsed(object? elapsedStorageTimer, ElapsedEventArgs eventArgs)
    {
        var itemWithElapsedStorageTimer = ItemWithElapsedStorageTimer(elapsedStorageTimer!);
        if (itemWithElapsedStorageTimer is not null)
        {
            if (Remove(itemWithElapsedStorageTimer))
                ItemStorageTimeElapsed?.Invoke(this, itemWithElapsedStorageTimer);
        }
    }

    TempStorageItem<T>? ItemWithElapsedStorageTimer(object elapsedStorageTimer)
    {
        try { return this.Single(item => Equals(item.Value, elapsedStorageTimer)); }
        catch { return null; }
    }

    internal static bool StorageTimeIsUnlimited(double storageTime) => storageTime <= 0;
}