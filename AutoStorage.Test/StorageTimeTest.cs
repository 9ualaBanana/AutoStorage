namespace TempStorage.Test;

public class StorageTimeTest
{
    [Fact]
    public void IsUnlimited_ShouldBe_TrueFor_Unlimited()
    {
        ((StorageTime)StorageTime.Unlimited).IsUnlimited.Should().BeTrue();
    }

    [Fact]
    public void Default_ShouldBe_IntervalDefaultExpression()
    {
        StorageTime.Default.Should().Be(default(Interval));
    }

    [Fact]
    public void StorageTimeOf_UnlimitedStorageTimer_ShouldBe_Unlimited()
    {
        var unlimitedStorageTimer = new StorageTimerFactory(TestData.StorageTime)
            .CreateWith(StorageTime.Unlimited, TestData.ElapsedEventHandlerStub);

        ((Interval?)StorageTime.Of(unlimitedStorageTimer)).Should().Be(StorageTime.Unlimited);
    }

    [Fact]
    public void StorageTimeOf_DefaultStorageTimer_ShouldBe_Default()
    {
        var defaultStorageTime = TestData.StorageTime;

        var defaultStorageTimer = new StorageTimerFactory(defaultStorageTime)
            .CreateWith(StorageTime.Default, TestData.ElapsedEventHandlerStub);

        StorageTime.Of(defaultStorageTimer).Should().Be(defaultStorageTime);
    }
}
