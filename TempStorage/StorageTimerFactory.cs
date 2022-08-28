using System.Timers;

namespace System.Collections.Specialized;

internal class StorageTimerFactory
{
    internal readonly StorageTime DefaultStorageTime;
    internal GTimer? DefaultStorageTimer(params ElapsedEventHandler[] with) => CreateWith(DefaultStorageTime, with);
    internal const GTimer? UnlimitedStorageTimer = StorageTimer.Unlimited;


    internal StorageTimerFactory(StorageTime? defaultStorageTime) =>
        DefaultStorageTime = defaultStorageTime ?? StorageTime.Unlimited;


    internal GTimer? CreateWith(StorageTime storageTime, params ElapsedEventHandler[] elapsedEventHandlers) =>
        (storageTime.IsUnlimited ? StorageTimer.Unlimited : storageTime.IsDefault ?
        DefaultStorageTimer(with: elapsedEventHandlers) : new GTimer((Interval)storageTime!)
        )?.Initialize(with: elapsedEventHandlers);
}
