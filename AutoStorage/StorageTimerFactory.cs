using System.Timers;

namespace System.Collections.Specialized;

internal class StorageTimerFactory
{
    internal readonly StorageTime DefaultStorageTime;
    internal static readonly StorageTimer UnlimitedStorageTimer = StorageTimer.Unlimited;
    internal StorageTimer DefaultStorageTimer => CreateWith(DefaultStorageTime);

    readonly ElapsedEventHandler _onElapsed;


    internal StorageTimerFactory(StorageTime? defaultStorageTime, ElapsedEventHandler onElapsed)
    {
        DefaultStorageTime = defaultStorageTime ?? StorageTime.Unlimited;
        _onElapsed = onElapsed;
    }


    internal StorageTimer CreateWith(StorageTime storageTime) =>
        (storageTime.IsUnlimited ? StorageTimer.Unlimited : storageTime.IsDefault ?
        DefaultStorageTimer : new StorageTimer(storageTime)
        ).InitializeWith(_onElapsed);
}
