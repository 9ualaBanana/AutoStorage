## Overview
**AutoStorage** is the collection that allows specifying **storage time** for elements stored in it. **Default storage time** that can be set when collection is initialized *(unlimited by default)* is assigned to each element unless **overriden** upon addition. When storage time of an element is elapsed an **event** with all the neccessary information about it including the element itself is raised.

### StorageTime
`AutoStorage<T>` uses `StorageTime` struct for representing the concept of time in **milliseconds**. It supports implicit conversion from any numeric value, `TimeSpan` and `Interval` which is another struct defined in my other [GTimer](https://github.com/9ualaBanana/GTimer "GTimer") library.
```C#
using System.Timers;

StorageTime fromTimeSpan = TimeSpan.FromMilliseconds(100);
StorageTime fromInterval = Interval.Default;
StorageTime fromInteger = 100;
StorageTime fromLong = 100L;
StorageTime fromDouble = 100d;
StorageTime fromFloat = 100f;
```
`StorageTime` itself defines some *special values*. Although they are backed by actual numeric values, they should be used only when working with `AutoStorage<T>` as that\'s the only meaningful context for them. These are `StorageTime.Unlimited` and `StorageTime.Default`. Instances of `StorageTime` should be checked for being equal to these values using following fields:
```C#
((StorageTime)StorageTime.Unlimited).IsUnlimited
((StorageTime)StorageTime.Default).IsDefault
```

### AutoStorageItem\<T>
`AutoStorageItem<T>` is the class used for internal representation of elements inside `AutoStorage<T>` with its equality contract based on the equality contract of `T`. It wraps the element of type `T` and `StorageTimer` used for monitoring its storage time. `StorageTimer` provides read-only access to some properties of `GTimer` that might be of any interest to the user. Instances of `AutoStorageItem` can only be obtained via `ItemStorageTimeElapsed` event which is defined as `EventHandler<AutoStorageItem<T>>`.

### AutoStorage\<T>
`AutoStorage<T>` defines constructors that take the same set of parameters that most collections do with the additional optional `StorageTime defaultStorageTime` parameter. Initializing `AutoStorage<T>` without explicitly providing `StorageTime` as the constructor argument sets its `DefaultStorageTime` to `StorageTime.Unlimited` which is assigned to any element that is added without specifying different one instead. Passing `StorageTime.Default` as the constructor argument is functionally equivalent to passing `StorageTime.Unlimited` or calling default constructor.
```C#
new AutoStorage<T>().DefaultStorageTime == new AutoStorage<T>(StorageTime.Default).DefaultStorageTime == new AutoStorage<T>(StorageTime.Unlimited).DefaultStorageTime;
```
But passing `StorageTime.Default` as one of the arguments to methods that populate `AutoStorage<T>` sets the storage time of the element being added to `DefaultStorageTime` of that `AutoStorage<T>` instance. Overloads that don\'t accept `StorageTime storageTime` as one of the arguments have the same behavior.
```C#
// Functionally the same.
autoStorage.Add(a);
autoStorage.Add(b, StorageTime.Default);
```

#### Methods
```C#
    public T AddOrUpdateValue(T value, T updatedValue, bool resetStorageTime = true)
    public void AddOrUpdateStorageTime(T value, StorageTime storageTime)
    public void AddOrResetStorageTime(T value)
    public void AddOrResetStorageTime(T value, StorageTime storageTime)
    public bool TryResetStorageTime(T value)
    public bool TryUpdateValue(T value, T updatedValue, bool resetStorageTime = true)
    public bool TryUpdateStorageTime(T value, StorageTime storageTime)
    public T GetOrAddValue(T value)
    public T GetOrAddValue(T value, StorageTime storageTime)
    public StorageTimer GetSorageTimerOrAdd(T value)
    public StorageTimer GetSorageTimerOrAdd(T value, StorageTime storageTime)
    public bool TryGetValue(T value, [MaybeNullWhen(false)] out T storedValue)
    public bool TryGetStorageTimer(T value, [MaybeNullWhen(false)] out StorageTimer storageTimer)
```

## Installation
[NuGet Download page](https://www.nuget.org/packages/AutoStorage "NuGet Download page")

For guidance on how to install NuGet packages refer [here](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-using-the-dotnet-cli) and [here](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio).

## Suggestions

If you would like to see some additional functionality that isn\'t provided by this library yet, feel free to leave your proposals in [**Issues**](https://github.com/9ualaBanana/AutoStorage/issues) section.  Any feedback is highly appreciated.