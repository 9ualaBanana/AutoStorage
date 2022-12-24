namespace TempStorage.Test;

public class StorageTimerTest
{
    [Fact]
    public void IsUnlimited_ShouldBe_TrueFor_UnlimitedStorageTimer()
    {
        var storageTimerFactory = new StorageTimerFactory(TestData.StorageTime, TestData.ElapsedEventHandlerStub);

        var unlimitedStorageTimer = storageTimerFactory
            .CreateWith(StorageTime.Unlimited);

        unlimitedStorageTimer.IsUnlimited.Should().BeTrue();
    }

    [Fact]
    public void InitializeWith_Should_StartStorageTimer()
    {
        var storageTimer = new StorageTimer(TestData.StorageTime);

        storageTimer.InitializeWith(TestData.ElapsedEventHandlerStub);

        storageTimer.Enabled.Should().BeTrue();
    }
}
