using System.Timers;

namespace System.Collections.Specialized;

internal class StorageTimerFactory
{
    internal readonly StorageTime DefaultStorageTime;
    internal StorageTimer DefaultStorageTimer(params ElapsedEventHandler[] with) => CreateWith(DefaultStorageTime, with);
    internal static readonly StorageTimer UnlimitedStorageTimer = StorageTimer.Unlimited;


    internal StorageTimerFactory(StorageTime? defaultStorageTime) =>
        DefaultStorageTime = defaultStorageTime ?? StorageTime.Unlimited;


    internal StorageTimer CreateWith(StorageTime storageTime, params ElapsedEventHandler[] elapsedEventHandlers) =>
        (storageTime.IsUnlimited ? StorageTimer.Unlimited : storageTime.IsDefault ?
        DefaultStorageTimer(with: elapsedEventHandlers) : new StorageTimer(storageTime)
        ).Initialize(with: elapsedEventHandlers);
}
