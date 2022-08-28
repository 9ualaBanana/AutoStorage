namespace TempStorage.Test;

public class StorageTimerFactoryTest
{
    [Fact]
    public void Constructor_SetsDefaultStorageTime()
    {
        var defaultStorageTime = TestData.StorageTime;

        var storageTimerFactory = new StorageTimerFactory(defaultStorageTime);

        storageTimerFactory.DefaultStorageTime.Should().Be(defaultStorageTime);
    }

    [Fact]
    public void CreateWith_UnlimitedStorageTime_ShouldReturn_UnlimitedStorageTimer()
    {
        var storageTimerFactory = new StorageTimerFactory(TestData.StorageTime);

        var unlimitedStorageTimer = storageTimerFactory
            .CreateWith(StorageTime.Unlimited, TestData.ElapsedEventHandlerStub);

        unlimitedStorageTimer.IsUnlimited.Should().BeTrue();
    }

    [Fact]
    public void CreateWith_DefaultStorageTime_ShouldReturn_TimerWithDefaultStorageTime()
    {
        var storageTimerFactory = new StorageTimerFactory(TestData.StorageTime);

        var timerWithDefaultStorageTime = storageTimerFactory
            .CreateWith(StorageTime.Default, TestData.ElapsedEventHandlerStub)!;

        timerWithDefaultStorageTime.Interval.Should().Be(storageTimerFactory.DefaultStorageTime);
    }

    [Fact]
    public void CreateWith_FiniteStorageTime_ShouldReturn_TimerWithThatStorageTime()
    {
        var finiteStorageTime = TestData.StorageTime;

        var finiteStorageTimer = new StorageTimerFactory(TestData.DifferentStorageTime)
            .CreateWith(finiteStorageTime, TestData.ElapsedEventHandlerStub)!;

        finiteStorageTimer.Interval.Should().Be(finiteStorageTime);
    }
}
