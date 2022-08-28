namespace TempStorage.Test;

internal class TestData
{
    internal static double FiniteStorageTime = 10_000;
    internal static double DifferentFiniteStorageTime = FiniteStorageTime + 10;
    internal static ElapsedEventHandler ElapsedEventHandlerStub = (_, _) => { };
}
