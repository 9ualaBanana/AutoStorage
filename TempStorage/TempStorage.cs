using System.Timers;

namespace System.Collections.Specialized;

/// <summary>
/// Collection that automatically removes items after their <see cref="StorageTime"/> is elapsed.
/// </summary>
/// <typeparam name="T">The type of stored items.</typeparam>
public class TempStorage<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISet<T>
{
    readonly HashSet<TempStorageItem<T>> _tempStorage;
    readonly StorageTimerFactory _storageTimerFactory;

    /// <summary>
    /// The storage time assigned to items by default when they are added without specifying one.
    /// </summary>
    public StorageTime DefaultStorageTime => _storageTimerFactory.DefaultStorageTime;
    /// <summary>
    /// Raised when some item is removed from <see cref="TempStorage{T}"/> due to elapsed storage time.
    /// </summary>
    public event EventHandler<TempStorageItem<T>>? ItemStorageTimeElapsed;


    #region Constructors
    /// <summary>
    /// Initializes empty <see cref="TempStorage{T}"/> with <see cref="DefaultStorageTime"/> set to <see cref="StorageTime.Unlimited"/>.
    /// </summary>
    public TempStorage() : this(EqualityComparer<TempStorageItem<T>>.Default)
    {
    }

    /// <summary>
    /// Initializes empty <see cref="TempStorage{T}"/> with <see cref="DefaultStorageTime"/> set to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public TempStorage(StorageTime defaultStorageTime)
        : this(EqualityComparer<TempStorageItem<T>>.Default, defaultStorageTime)
    {
    }

    /// <summary>
    /// Initializes empty <see cref="TempStorage{T}"/> that uses the specified equality comparer for the storage type
    /// and sets <see cref="DefaultStorageTime"/> to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="comparer">The <see cref="EqualityComparer{T}"/> implementation to use when comparing values in the storage,
    /// or null to use the default <see cref="EqualityComparer{T}"/> implementation for the storage type.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public TempStorage(IEqualityComparer<TempStorageItem<T>>? comparer, StorageTime? defaultStorageTime = null)
    {
        _storageTimerFactory = new StorageTimerFactory(defaultStorageTime);
        _tempStorage = new(comparer);
    }


    /// <summary>
    /// Initializes <see cref="TempStorage{T}"/> that is empty, but has reserved space for <paramref name="capacity"/> items
    /// and sets <see cref="DefaultStorageTime"/> to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="capacity">The initial size of the <see cref="TempStorage{T}"/>.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public TempStorage(int capacity, StorageTime? defaultStorageTime = null)
        : this(capacity, null, defaultStorageTime)
    {
    }

    /// <summary>
    /// Initializes <see cref="TempStorage{T}"/> that is empty, but has reserved space for <paramref name="capacity"/> items
    /// that uses the specified equality comparer for the storage type and sets <see cref="DefaultStorageTime"/>
    /// to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="capacity">The initial size of the <see cref="TempStorage{T}"/>.</param>
    /// <param name="comparer">The <see cref="EqualityComparer{T}"/> implementation to use when comparing values in the storage,
    /// or null to use the default <see cref="EqualityComparer{T}"/> implementation for the storage type.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public TempStorage(int capacity, IEqualityComparer<TempStorageItem<T>>? comparer, StorageTime? defaultStorageTime = null)
    {
        _storageTimerFactory = new StorageTimerFactory(defaultStorageTime);
        _tempStorage = new(capacity, comparer);
    }


    /// <summary>
    /// Initializes <see cref="TempStorage{T}"/> from <paramref name="collection"/> with <see cref="DefaultStorageTime"/>
    /// set to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the <see cref="TempStorage{T}"/>.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public TempStorage(IEnumerable<T> collection, StorageTime? defaultStorageTime = null)
        : this(collection, null, defaultStorageTime)
    {
    }

    /// <summary>
    /// Initializes <see cref="TempStorage{T}"/> from <paramref name="collection"/> aht uses the specified equality comparer
    /// for the storage type and sets <see cref="DefaultStorageTime"/> to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the <see cref="TempStorage{T}"/>.</param>
    /// <param name="comparer">The <see cref="EqualityComparer{T}"/> implementation to use when comparing values in the storage,
    /// or null to use the default <see cref="EqualityComparer{T}"/> implementation for the storage type.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public TempStorage(IEnumerable<T> collection, IEqualityComparer<TempStorageItem<T>>? comparer, StorageTime? defaultStorageTime = null)
    {
        _storageTimerFactory = new StorageTimerFactory(defaultStorageTime);
        _tempStorage = new(collection.Select(
            element => new TempStorageItem<T>(element, _storageTimerFactory.DefaultStorageTimer(with: OnStorageTimeElapsed))),
            comparer);
    }
    #endregion


    /// <summary>
    /// Adds <paramref name="item"/> to <see cref="TempStorage{T}"/> or resets its storage time.
    /// </summary>
    /// <param name="item">The item to add or whose storage time to reset.</param>
    public void AddOrResetStorageTime(T item) => AddOrUpdateStorageTime(
        new TempStorageItem<T>(item, _storageTimerFactory.DefaultStorageTimer(with: OnStorageTimeElapsed)),
        updateStorageTimer: false);

    /// <summary>
    /// Adds <paramref name="item"/>  to <see cref="TempStorage{T}"/> with the specified <paramref name="storageTime"/>.
    /// </summary>
    /// <param name="item">The item to add or whose storage time to update to <paramref name="storageTime"/>.</param>
    /// <param name="storageTime">The storage time to update item's storage time to.</param>
    public void AddOrUpdateStorageTime(T item, StorageTime storageTime) =>
        AddOrUpdateStorageTime(new TempStorageItem<T>(item, _storageTimerFactory.CreateWith(storageTime, OnStorageTimeElapsed)));

    void AddOrUpdateStorageTime(TempStorageItem<T> item, bool updateStorageTimer = true)
    {
        if (!TryUpdate(item, updateStorageTimer))
            Add(item);
    }


    /// <summary>
    /// Updates <paramref name="item"/>'s storage time to <paramref name="storageTime"/> if it is in storage.
    /// </summary>
    /// <param name="item">The item to add or whose storage time to update to <paramref name="storageTime"/>.</param>
    /// <param name="storageTime">The storage time to update item's storage time to.</param>
    /// <returns><i>true</i> if item's storage time is successfully updated; <i>false</i> if item is not in the storage.</returns>
    public bool TryUpdate(T item, StorageTime storageTime) => TryUpdate(
        new(item, _storageTimerFactory.CreateWith(storageTime, OnStorageTimeElapsed)),
        updateStorageTimer: true);

    bool TryUpdate(TempStorageItem<T> item, bool updateStorageTimer)
    {
        if (_tempStorage.TryGetValue(item, out var storedItem))
        { Update(storedItem, item, updateStorageTimer); return true; }

        else return false;
    }

    /// <summary>
    /// Restarts the storage timer of <paramref name="storedItem"/> or updates it with the one provided by <paramref name="newItem"/>.
    /// </summary>
    /// <param name="storedItem">The item from the storage to update.</param>
    /// <param name="newItem">The new item containing data to update <paramref name="storedItem"/> with.</param>
    /// <param name="updateStorageTimer">Specifies whether <paramref name="storedItem"/>'s timer should be changed with <paramref name="newItem"/>'s one.</param>
    static void Update(TempStorageItem<T> storedItem, TempStorageItem<T> newItem, bool updateStorageTimer)
    {
        if (updateStorageTimer) storedItem.Timer = newItem.Timer;
        storedItem.Timer.Restart();
    }

    #region ICollection<T>
    void ICollection<T>.Add(T item) => Add(item, StorageTime.Default);

    public void Clear() => _tempStorage.Clear();

    public bool Contains(T item) => _tempStorage.Contains(new(item));

    public void CopyTo(T[] array, int arrayIndex) => _tempStorage.Values().ToArray().CopyTo(array, arrayIndex);

    /// <inheritdoc cref="Remove(T)"/>
    bool Remove(TempStorageItem<T> item) => Remove(item.Value);

    /// <summary>
    /// Removes the specified item from a <see cref="TempStorage{T}"/>.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>
    /// <i>true</i> if the item is successfully found and removed; otherwise, <i>false</i>. This method returns
    /// <i>false</i> if <paramref name="item"/> is not found in the <see cref="TempStorage{T}"/> object.
    /// </returns>
    public bool Remove(T item) => _tempStorage.Remove(new TempStorageItem<T>(item));

    public int Count => _tempStorage.Count;

    bool ICollection<T>.IsReadOnly => false;
    #endregion

    #region ISet<T>
    /// <summary>
    /// Adds an item to the storage and returns a value to indicate if the item was successfully added.
    /// </summary>
    /// <param name="item">The item to add to the storage.</param>
    /// <returns>
    /// <i>true</i> if the item is added to the <see cref="TempStorage{T}"/>;
    /// <i>false</i> if the item is already present.
    /// </returns>
    public bool Add(T item) => _tempStorage.Add(
        new(item, _storageTimerFactory.DefaultStorageTimer(with: OnStorageTimeElapsed))
        );

    /// <summary>
    /// Adds an item with the specified <paramref name="storageTime"/> to the storage
    /// and returns a value to indicate if the item was successfully added.
    /// </summary>
    /// <param name="item">The item to add to the storage.</param>
    /// <param name="storageTime">The storage time with which the item is added</param>
    /// <returns>
    /// <i>true</i> if the item is added to the <see cref="TempStorage{T}"/>;
    /// <i>false</i> if the item is already present.
    /// </returns>
    public bool Add(T item, StorageTime storageTime) =>
        _tempStorage.Add(
            new TempStorageItem<T>(item, _storageTimerFactory.CreateWith(storageTime, OnStorageTimeElapsed))
            );

    /// <summary>
    /// Adds an item to a storage and returns a value to indicate if the item was successfully added.
    /// </summary>
    /// <remarks>
    /// Only items with already initialized storage timers must be passed.
    /// </remarks>
    /// <param name="item">The item to add to the storage.</param>
    /// <returns>
    /// <i>true</i> if the item is added to the <see cref="TempStorage{T}"/>;
    /// <i>false</i> if the item is already present.
    /// </returns>
    bool Add(TempStorageItem<T> item) => _tempStorage.Add(item);

    public void ExceptWith(IEnumerable<T> other) => _tempStorage.ExceptWith(other.AsTempStorageItems());

    public void IntersectWith(IEnumerable<T> other) => _tempStorage.IntersectWith(other.AsTempStorageItems());

    public bool IsProperSubsetOf(IEnumerable<T> other) => _tempStorage.IsProperSubsetOf(other.AsTempStorageItems());

    public bool IsProperSupersetOf(IEnumerable<T> other) => _tempStorage.IsProperSupersetOf(other.AsTempStorageItems());

    public bool IsSubsetOf(IEnumerable<T> other) => _tempStorage.IsSubsetOf(other.AsTempStorageItems());

    public bool IsSupersetOf(IEnumerable<T> other) => _tempStorage.IsSupersetOf(other.AsTempStorageItems());

    public bool Overlaps(IEnumerable<T> other) => _tempStorage.Overlaps(other.AsTempStorageItems());

    public bool SetEquals(IEnumerable<T> other) => _tempStorage.SetEquals(other.AsTempStorageItems());

    public void SymmetricExceptWith(IEnumerable<T> other) => _tempStorage.SymmetricExceptWith(other.AsTempStorageItems());

    public void UnionWith(IEnumerable<T> other) => _tempStorage.UnionWith(other.AsTempStorageItems());
    #endregion

    #region Enumeration
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator() => _tempStorage.Values().GetEnumerator();
    #endregion

    void OnStorageTimeElapsed(object? elapsedStorageTimer, ElapsedEventArgs eventArgs)
    {
        if (elapsedStorageTimer is null) throw new ArgumentNullException(
                nameof(elapsedStorageTimer),
                "Storage timer must provide reference to itself when raising the event upon its elapse.");

        var itemWithElapsedStorageTimer = _tempStorage.ItemWith(elapsedStorageTimer!);
        if (itemWithElapsedStorageTimer is not null)
        {
            if (Remove(itemWithElapsedStorageTimer))
            {
                itemWithElapsedStorageTimer.Timer.Elapsed -= OnStorageTimeElapsed;
                ItemStorageTimeElapsed?.Invoke(this, itemWithElapsedStorageTimer);
            }
        }
    }

    // Used exclusively for testing purposes.
    internal TempStorageItem<T> this[T value]
    {
        get
        {
            _ = _tempStorage.TryGetValue(new TempStorageItem<T>(value), out var item);
            return item!;
        }
    }
}
