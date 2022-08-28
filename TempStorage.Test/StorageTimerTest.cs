namespace TempStorage.Test;

public class StorageTimerTest
{
    [Fact]
    public void IsUnlimited_ShouldBe_TrueFor_UnlimitedStorageTimer()
    {
        var storageTimerFactory = new StorageTimerFactory(TestData.StorageTime);

        var unlimitedStorageTimer = storageTimerFactory
            .CreateWith(StorageTime.Unlimited, TestData.ElapsedEventHandlerStub);

        unlimitedStorageTimer.IsUnlimited.Should().BeTrue();
    }

    [Fact]
    public void Initialize_ShouldThrow_WhenNoElapsedEventHandlersAreProvided()
    {
        Action initialization = () => ((StorageTimer)new GTimer()).Initialize();

        initialization.Should().Throw<MissingMethodException>();
    }

    [Fact]
    public void Initialize_Should_StartStorageTimer()
    {
        var storageTimer = (StorageTimer)new GTimer();

        storageTimer.Initialize(TestData.ElapsedEventHandlerStub);

        ((GTimer)storageTimer!).Enabled.Should().BeTrue();
    }
}
