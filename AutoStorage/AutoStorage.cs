using System.Diagnostics.CodeAnalysis;
using System.Timers;

namespace System.Collections.Specialized;

/// <summary>
/// Collection that automatically removes items after their <see cref="StorageTime"/> is elapsed.
/// </summary>
/// <typeparam name="T">The type of stored items.</typeparam>
public class AutoStorage<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISet<T>
{
    readonly HashSet<AutoStorageItem<T>> _tempStorage;
    readonly StorageTimerFactory _storageTimerFactory;

    /// <summary>
    /// The storage time assigned to items by default when they are added without specifying one.
    /// </summary>
    public StorageTime DefaultStorageTime => _storageTimerFactory.DefaultStorageTime;
    /// <summary>
    /// Raised when some item is removed from <see cref="AutoStorage{T}"/> due to elapsed storage time.
    /// </summary>
    public event EventHandler<AutoStorageItem<T>>? ItemStorageTimeElapsed;


    #region Constructors
    /// <summary>
    /// Initializes empty <see cref="AutoStorage{T}"/> with <see cref="DefaultStorageTime"/> set to <see cref="StorageTime.Unlimited"/>.
    /// </summary>
    public AutoStorage() : this(EqualityComparer<AutoStorageItem<T>>.Default)
    {
    }

    /// <summary>
    /// Initializes empty <see cref="AutoStorage{T}"/> with <see cref="DefaultStorageTime"/> set to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public AutoStorage(StorageTime defaultStorageTime)
        : this(EqualityComparer<AutoStorageItem<T>>.Default, defaultStorageTime)
    {
    }

    /// <summary>
    /// Initializes empty <see cref="AutoStorage{T}"/> that uses the specified equality comparer for the storage type
    /// and sets <see cref="DefaultStorageTime"/> to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="comparer">The <see cref="EqualityComparer{T}"/> implementation to use when comparing values in the storage,
    /// or null to use the default <see cref="EqualityComparer{T}"/> implementation for the storage type.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public AutoStorage(IEqualityComparer<AutoStorageItem<T>>? comparer, StorageTime? defaultStorageTime = null)
    {
        _storageTimerFactory = new StorageTimerFactory(defaultStorageTime);
        _tempStorage = new(comparer);
    }


    /// <summary>
    /// Initializes <see cref="AutoStorage{T}"/> that is empty, but has reserved space for <paramref name="capacity"/> items
    /// and sets <see cref="DefaultStorageTime"/> to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="capacity">The initial size of the <see cref="AutoStorage{T}"/>.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public AutoStorage(int capacity, StorageTime? defaultStorageTime = null)
        : this(capacity, null, defaultStorageTime)
    {
    }

    /// <summary>
    /// Initializes <see cref="AutoStorage{T}"/> that is empty, but has reserved space for <paramref name="capacity"/> items
    /// that uses the specified equality comparer for the storage type and sets <see cref="DefaultStorageTime"/>
    /// to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="capacity">The initial size of the <see cref="AutoStorage{T}"/>.</param>
    /// <param name="comparer">The <see cref="EqualityComparer{T}"/> implementation to use when comparing values in the storage,
    /// or null to use the default <see cref="EqualityComparer{T}"/> implementation for the storage type.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public AutoStorage(int capacity, IEqualityComparer<AutoStorageItem<T>>? comparer, StorageTime? defaultStorageTime = null)
    {
        _storageTimerFactory = new StorageTimerFactory(defaultStorageTime);
        _tempStorage = new(capacity, comparer);
    }


    /// <summary>
    /// Initializes <see cref="AutoStorage{T}"/> from <paramref name="collection"/> with <see cref="DefaultStorageTime"/>
    /// set to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the <see cref="AutoStorage{T}"/>.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public AutoStorage(IEnumerable<T> collection, StorageTime? defaultStorageTime = null)
        : this(collection, null, defaultStorageTime)
    {
    }

    /// <summary>
    /// Initializes <see cref="AutoStorage{T}"/> from <paramref name="collection"/> aht uses the specified equality comparer
    /// for the storage type and sets <see cref="DefaultStorageTime"/> to <paramref name="defaultStorageTime"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the <see cref="AutoStorage{T}"/>.</param>
    /// <param name="comparer">The <see cref="EqualityComparer{T}"/> implementation to use when comparing values in the storage,
    /// or null to use the default <see cref="EqualityComparer{T}"/> implementation for the storage type.</param>
    /// <param name="defaultStorageTime">The storage time assigned to items by default when they are added without specifying one.</param>
    public AutoStorage(IEnumerable<T> collection, IEqualityComparer<AutoStorageItem<T>>? comparer, StorageTime? defaultStorageTime = null)
    {
        _storageTimerFactory = new StorageTimerFactory(defaultStorageTime);
        _tempStorage = new(collection.Select(
            element => new AutoStorageItem<T>(element, _storageTimerFactory.DefaultStorageTimer(with: OnStorageTimeElapsed))),
            comparer);
    }
    #endregion


    /// <summary>
    /// If value is not in storage, adds <paramref name="value"/> to the storage with the <see cref="DefaultStorageTime"/>
    /// or provided <paramref name="storageTime"/> if <see cref="StorageTimeUse.OnAdd"/> is set.
    /// If value is in storage, updates <paramref name="value"/> to <paramref name="updatedValue"/> and also
    /// its storage timer to <paramref name="storageTime"/> if <see cref="StorageTimeUse.OnUpdate"/> is set.
    /// Optionally resets the storage timer when <see cref="StorageTimeUse.OnUpdate"/> is not set.
    /// </summary>
    /// <param name="value">The value to add or update.</param>
    /// <param name="updatedValue">The value to update to.</param>
    /// <param name="storageTime">The storage time to use when adding the value to the storage.</param>
    /// <param name="storageTimeUse">Specifies in which cases to use provided <paramref name="storageTime"/>.</param>
    /// <param name="resetStorageTime">Specifies whether to reset value's storage time upon update; when <see cref="StorageTimeUse.OnUpdate"/> flag is set, it is reset automatically.</param>
    public void AddOrUpdateItem(T value, T updatedValue, StorageTime storageTime, StorageTimeUse storageTimeUse, bool resetStorageTime = true)
    {
        if (_TryGetItem(value, out var storedItem))
        {
            storedItem.Value = updatedValue;
            if (storageTimeUse.HasFlag(StorageTimeUse.OnUpdate))
                storedItem.Timer = _storageTimerFactory.CreateWith(storageTime, OnStorageTimeElapsed);
            else if (resetStorageTime) storedItem.Timer.Restart();
        }
        else
        {
            if (storageTimeUse.HasFlag(StorageTimeUse.OnAdd))
                Add(value, storageTime);
            else Add(value);
        }
    }

    /// <summary>
    /// Adds <paramref name="value"/> to the storage with the <see cref="DefaultStorageTime"/> or updates it to <paramref name="updatedValue"/>.
    /// </summary>
    /// <param name="value">The value to add or update.</param>
    /// <param name="updatedValue">The value to update to.</param>
    /// <param name="resetStorageTime">Specifies whether to reset value's storage time upon update.</param>
    public void AddOrUpdateValue(T value, T updatedValue, bool resetStorageTime = true)
    {
        if (!TryUpdateValue(value, updatedValue, resetStorageTime))
            Add(value);
    }

    /// <summary>
    /// Updates the <paramref name="value"/> to <paramref name="updatedValue"/> if it is in storage and optionally resets its storage timer.
    /// </summary>
    /// <param name="value">The value to update.</param>
    /// <param name="updatedValue">The value to update to.</param>
    /// <param name="resetStorageTime">Specifies whether to reset value's storage time upon update.</param>
    /// <returns>
    /// <see langword="true"/> if the value is successfully updated; <see langword="false"/> if the value is not in the storage.
    /// </returns>
    public bool TryUpdateValue(T value, T updatedValue, bool resetStorageTime = true)
    {
        if (_TryGetItem(value, out var storedItem))
        {
            storedItem.Value = updatedValue;
            if (resetStorageTime) storedItem.Timer.Restart();
            return true;
        }

        else return false;
    }

    /// <summary>
    /// Updates <paramref name="value"/>'s storage time to <paramref name="storageTime"/> if it is in storage.
    /// </summary>
    /// <param name="value">The value to add or whose storage time to update to <paramref name="storageTime"/>.</param>
    /// <param name="storageTime">The storage time to update value's storage time to.</param>
    /// <returns>
    /// <see langword="true"/> if value's storage time is successfully updated;
    /// <see langword="false"/> if the value is not in the storage.
    /// </returns>
    public bool TryUpdateStorageTime(T value, StorageTime storageTime)
    {
        if (_TryGetItem(value, out var storedItem))
        {
            storedItem.Timer = _storageTimerFactory.CreateWith(storageTime, OnStorageTimeElapsed);
            return true;
        }

        else return false;
    }

    /// <summary>
    /// Adds <paramref name="value"/> with <see cref="DefaultStorageTime"/> to <see cref="AutoStorage{T}"/> or resets its storage time.
    /// </summary>
    /// <param name="value">The value to add or whose storage time to reset.</param>
    public void AddOrResetStorageTime(T value)
    {
        // TODO: Optimize as it currently creates new AutoSTorageItem twice.
        if (!TryResetStorageTime(value))
            Add(value);
    }

    /// <summary>
    /// Adds <paramref name="value"/> with <paramref name="storageTime"/> to <see cref="AutoStorage{T}"/> or resets its storage time.
    /// </summary>
    /// <param name="value">The value to add or whose storage time to reset.</param>
    /// <param name="storageTime">The storage time to add the item with.</param>
    public void AddOrResetStorageTime(T value, StorageTime storageTime)
    {
        if (!TryResetStorageTime(value))
            Add(value, storageTime);
    }

    /// <summary>
    /// Adds <paramref name="value"/>  to <see cref="AutoStorage{T}"/> with the specified <paramref name="storageTime"/>.
    /// </summary>
    /// <param name="value">The value to add or whose storage time to update to <paramref name="storageTime"/>.</param>
    /// <param name="storageTime">The storage time to update value's storage time to.</param>
    public void AddOrUpdateStorageTime(T value, StorageTime storageTime) =>
        _AddOrUpdateStorageTime(new AutoStorageItem<T>(value, _storageTimerFactory.CreateWith(storageTime, OnStorageTimeElapsed)));

    /// <summary>
    /// Resets value's storage time if it is in storage.
    /// </summary>
    /// <param name="value">The value whose storage time to reset.</param>
    /// <returns>
    /// <see langword="true"/> if value's storage time is successfully reset;
    /// <see langword="false"/> if the value is not in the storage.
    /// </returns>
    public bool TryResetStorageTime(T value)
    {
        if (TryGetStorageTimer(value, out var storageTimer))
        { storageTimer.Restart(); return true; }

        else return false;
    }

    /// <summary>
    /// Retrieves <paramref name="value"/> if it is in storage.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <param name="storedValue">The value from the storage that the search found
    /// or the default value of <typeparamref name="T"/> if the search yielded no result.</param>
    /// <returns>The value indicating whether the search was successful.</returns>
    public bool TryGetValue(T value, [MaybeNullWhen(false)] out T storedValue)
    {
        storedValue = default;

        if (_tempStorage.TryGetValue(new(value), out var storedItem))
        { storedValue = storedItem.Value; return true; }

        else return false;
    }

    /// <summary>
    /// Retrieves the storage timer of <paramref name="value"/> if it is in storage.
    /// </summary>
    /// <param name="value">The vlaue whose storage timer to get.</param>
    /// <param name="storageTimer">The storage timer of the <paramref name="value"/>.</param>
    /// <returns>The value indicating whether value's storage timer is successfully retrieved.</returns>
    public bool TryGetStorageTimer(T value, [MaybeNullWhen(false)] out StorageTimer storageTimer)
    {
        storageTimer = default;

        if (_TryGetItem(value, out var storedItem))
        { storageTimer = storedItem.Timer; return true; }

        else return false;
    }


    #region InternalStorageInterface
    // TODO: Add _AddOrReetStorageTime but need to show somehow that Timer property of that AutoStorageItem is irrelevant.
    void _AddOrUpdateStorageTime(AutoStorageItem<T> item)
    {
        if (!_TryUpdateStorageTime(item))
            _Add(item);
    }

    bool _TryUpdateStorageTime(AutoStorageItem<T> item)
    {
        if (_TryGetItem(item, out var storedItem))
        { storedItem.Timer = item.Timer; return true; }

        else return false;
    }

    bool _TryGetItem(T value, [MaybeNullWhen(false)] out AutoStorageItem<T> storedItem) =>
        _TryGetItem(new AutoStorageItem<T>(value), out storedItem);


    bool _TryGetItem(AutoStorageItem<T> item, [MaybeNullWhen(false)] out AutoStorageItem<T> storedItem) =>
        _tempStorage.TryGetValue(item, out storedItem);

    bool _Add(AutoStorageItem<T> item)
    {
        if (item.Timer.IsInitialized) { _tempStorage.Add(item); return true; }

        else throw new ArgumentException(
            $"{nameof(AutoStorageItem<T>)}'s storage timer must be initialized before adding it to the storage.",
            nameof(item));
    }
    #endregion

    #region ICollection<T>
    void ICollection<T>.Add(T item) => Add(item, StorageTime.Default);

    public void Clear() => _tempStorage.Clear();

    public bool Contains(T item) => _tempStorage.Contains(new(item));

    public void CopyTo(T[] array, int arrayIndex) => _tempStorage.Values().ToArray().CopyTo(array, arrayIndex);

    /// <inheritdoc cref="Remove(T)"/>
    bool Remove(AutoStorageItem<T> item) => Remove(item.Value);

    /// <summary>
    /// Removes the specified item from a <see cref="AutoStorage{T}"/>.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>
    /// <i>true</i> if the item is successfully found and removed; otherwise, <i>false</i>. This method returns
    /// <i>false</i> if <paramref name="item"/> is not found in the <see cref="AutoStorage{T}"/> object.
    /// </returns>
    public bool Remove(T item) => _tempStorage.Remove(new AutoStorageItem<T>(item));

    public int Count => _tempStorage.Count;

    bool ICollection<T>.IsReadOnly => false;
    #endregion

    #region ISet<T>
    /// <summary>
    /// Adds an item to the storage and returns a value to indicate if the item was successfully added.
    /// </summary>
    /// <param name="value">The item to add to the storage.</param>
    /// <returns>
    /// <i>true</i> if the item is added to the <see cref="AutoStorage{T}"/>;
    /// <i>false</i> if the item is already present.
    /// </returns>
    public bool Add(T value) =>
        _Add(new(value, _storageTimerFactory.DefaultStorageTimer(with: OnStorageTimeElapsed)));

    /// <summary>
    /// Adds an item with the specified <paramref name="storageTime"/> to the storage
    /// and returns a value to indicate if the item was successfully added.
    /// </summary>
    /// <param name="value">The item to add to the storage.</param>
    /// <param name="storageTime">The storage time with which the item is added</param>
    /// <returns>
    /// <i>true</i> if the item is added to the <see cref="AutoStorage{T}"/>;
    /// <i>false</i> if the item is already present.
    /// </returns>
    public bool Add(T value, StorageTime storageTime) =>
        _Add(new(value, _storageTimerFactory.CreateWith(storageTime, OnStorageTimeElapsed)));

    /// <summary>
    /// Adds an item to a storage and returns a value to indicate if the item was successfully added.
    /// </summary>
    /// <remarks>
    /// Only items with already initialized storage timers must be passed.
    /// </remarks>
    /// <param name="item">The item to add to the storage.</param>
    /// <returns>
    /// <i>true</i> if the item is added to the <see cref="AutoStorage{T}"/>;
    /// <i>false</i> if the item is already present.
    /// </returns>

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
}
