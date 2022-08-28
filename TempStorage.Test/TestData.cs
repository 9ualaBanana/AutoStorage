namespace TempStorage.Test;

internal class TestData
{
    internal static double StorageTime = 500_000;
    internal static double DifferentStorageTime = StorageTime + 10;
    internal static ElapsedEventHandler ElapsedEventHandlerStub = (_, _) => { };
}
