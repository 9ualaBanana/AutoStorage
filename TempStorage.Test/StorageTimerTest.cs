namespace TempStorage.Test;

public class StorageTimerTest
{
    [Fact]
    public void IsUnlimited_ShouldBe_TrueFor_UnlimitedStorageTimer()
    {
        var storageTimerFactory = new StorageTimerFactory(TestData.StorageTime);

        var unlimitedStorageTimer = storageTimerFactory
            .CreateWith(StorageTime.Unlimited, TestData.ElapsedEventHandlerStub);

        StorageTimer.IsUnlimited(unlimitedStorageTimer).Should().BeTrue();
    }

    [Fact]
    public void Initialize_ShouldThrowWhen_NoElapsedEventHandlersAreProvided()
    {
        Action initialization = () => new GTimer().Initialize();

        initialization.Should().Throw<MissingMethodException>();
    }

    [Fact]
    public void Initialize_Should_StartStorageTimer()
    {
        var storageTimer = new GTimer();

        storageTimer.Initialize(TestData.ElapsedEventHandlerStub);

        storageTimer.Enabled.Should().BeTrue();
    }
}
