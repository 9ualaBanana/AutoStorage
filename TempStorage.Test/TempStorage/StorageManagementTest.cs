namespace TempStorage.Test.TempStorage;

public class StorageManagementTest
{
    [Fact]
    public void TempStorageItem_IsRemoved_FromTempStorage_WhenStorageTimeIsElapsed()
    {
        var value = new Random().Next();
        var tempStorage = new TempStorage<int> { { value, 3000 } };

        tempStorage.Contains(value).Should().BeTrue();

        using var tempStorageEventSource = tempStorage.Monitor();
        Thread.Sleep(3000);
        tempStorageEventSource.Should().Raise(nameof(tempStorage.ItemStorageTimeElapsed))
            .WithSender(tempStorage)
            .WithArgs<TempStorageItem<int>>(item => !tempStorage.Contains(item.Value));
    }

    [Fact]
    public void TryUpdate_RestartsStorageTimer_IfItemIsInStorage()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next(); tempStorage.Add(value);

        var item = tempStorage[value];
        item.Should().NotBeNull();
        item.Timer!.Interval.Should().Be(TestData.StorageTime);

        tempStorage.TryUpdate(value, TestData.DifferentStorageTime).Should().BeTrue();

        item = tempStorage[value];
        item.Timer!.Interval.Should().Be(TestData.DifferentStorageTime);
        item.Timer!.LastResetTime.Should().NotBe(item.Timer.CreationTime);
    }

    [Fact]
    public void AddOrResetStorageTime_ResetsCurrentStorageTimer()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next();
        tempStorage.AddOrResetStorageTime(value);

        tempStorage.Contains(value).Should().BeTrue();

        tempStorage.AddOrResetStorageTime(value);

        var item = tempStorage[value];
        item.Timer!.LastResetTime.Should().NotBe(item.Timer.CreationTime);
    }

    [Fact]
    public void AddOrUpdateStorageTime_UpdatesCurrentStorageTimer()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next();
        tempStorage.Add(value);

        tempStorage.AddOrUpdateStorageTime(value, TestData.DifferentStorageTime);

        tempStorage[value].Timer!.Interval.Should().Be(TestData.DifferentStorageTime);
    }

    [Fact]
    public void AddOrUpdateStorageTime_AddsNewElement_WithProvidedStorageTime()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next();
        tempStorage.AddOrUpdateStorageTime(value, TestData.DifferentStorageTime);

        tempStorage[value].Timer!.Interval.Should().Be(TestData.DifferentStorageTime);
    }
}
