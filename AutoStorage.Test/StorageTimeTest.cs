namespace TempStorage.Test;

public class StorageTimeTest
{
    [Fact]
    public void IsUnlimited_ShouldBe_TrueFor_Unlimited()
    {
        ((StorageTime)StorageTime.Unlimited).IsUnlimited.Should().BeTrue();
    }
    
    [Fact]
    public void IsDefault_ShouldBe_TrueFor_Default()
    {
        ((StorageTime)StorageTime.Default).IsDefault.Should().BeTrue();
    }

    [Fact]
    public void Default_ShouldBe_IntervalDefaultExpression()
    {
        StorageTime.Default.Should().Be(default(Interval));
        StorageTime.Default.Should().Be((Interval)0);
    }

    [Fact]
    public void StorageTimeOf_UnlimitedStorageTimer_ShouldBe_Unlimited()
    {
        var unlimitedStorageTimer = new StorageTimerFactory(TestData.StorageTime, TestData.ElapsedEventHandlerStub)
            .CreateWith(StorageTime.Unlimited);

        ((Interval?)unlimitedStorageTimer.Interval).Should().Be(StorageTime.Unlimited);
    }

    [Fact]
    public void StorageTimeOf_DefaultStorageTimer_ShouldBe_Default()
    {
        var defaultStorageTime = TestData.StorageTime;

        var defaultStorageTimer = new StorageTimerFactory(defaultStorageTime, TestData.ElapsedEventHandlerStub)
            .CreateWith(StorageTime.Default);

        defaultStorageTimer.Interval.Should().Be(defaultStorageTime);
    }
}
