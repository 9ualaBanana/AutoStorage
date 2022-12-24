namespace TempStorage.Test;

public class StorageTimerFactoryTest
{
    [Fact]
    public void Constructor_SetsDefaultStorageTime()
    {
        var defaultStorageTime = TestData.StorageTime;

        var storageTimerFactory = new StorageTimerFactory(defaultStorageTime, TestData.ElapsedEventHandlerStub);

        storageTimerFactory.DefaultStorageTime.Should().Be(defaultStorageTime);
    }

    [Fact]
    public void CreateWith_UnlimitedStorageTime_ShouldReturn_UnlimitedStorageTimer()
    {
        var storageTimerFactory = new StorageTimerFactory(TestData.StorageTime, TestData.ElapsedEventHandlerStub);

        var unlimitedStorageTimer = storageTimerFactory
            .CreateWith(StorageTime.Unlimited);

        unlimitedStorageTimer.IsUnlimited.Should().BeTrue();
    }

    [Fact]
    public void CreateWith_DefaultStorageTime_ShouldReturn_TimerWithDefaultStorageTime()
    {
        var storageTimerFactory = new StorageTimerFactory(TestData.StorageTime, TestData.ElapsedEventHandlerStub);

        var timerWithDefaultStorageTime = storageTimerFactory
            .CreateWith(StorageTime.Default)!;

        timerWithDefaultStorageTime.Interval.Should().Be(storageTimerFactory.DefaultStorageTime);
    }

    [Fact]
    public void CreateWith_FiniteStorageTime_ShouldReturn_TimerWithThatStorageTime()
    {
        var finiteStorageTime = TestData.StorageTime;

        var finiteStorageTimer = new StorageTimerFactory(TestData.DifferentStorageTime, TestData.ElapsedEventHandlerStub)
            .CreateWith(finiteStorageTime)!;

        finiteStorageTimer.Interval.Should().Be(finiteStorageTime);
    }

    [Fact]
    public void PassingStorageTimeDefaultToConstructor_Should_SetDefaultStorageTimeTo_StorageTimeUnlimited()
    {
        var storageTimeFactory = new StorageTimerFactory(StorageTime.Default, TestData.ElapsedEventHandlerStub);

        storageTimeFactory.DefaultStorageTime.IsUnlimited.Should().BeTrue();
    }
}
