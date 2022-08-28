namespace TempStorage.Test;

internal class TestData
{
    internal static StorageTime ShortStorageTime = 100;
    internal static StorageTime StorageTime = 500_000;
    internal static StorageTime DifferentStorageTime = StorageTime + 10;
    internal static ElapsedEventHandler ElapsedEventHandlerStub = (_, _) => { };
}
