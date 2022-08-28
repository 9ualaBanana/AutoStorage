﻿namespace TempStorage.Test.TempStorage;

public class StorageManagementTest
{
    [Fact]
    public void TempStorageItem_IsRemoved_FromTempStorage_WhenStorageTimeIsElapsed()
    {
        var tempStorage = new TempStorage<int>();
        var value = new Random().Next();

        tempStorage.Add(value, TestData.ShortStorageTime);

        tempStorage.Contains(value).Should().BeTrue();
        value.ShouldBeRemovedFrom(tempStorage, after: (Interval)TestData.ShortStorageTime!);
    }

    [Fact]
    public void TryUpdate_RestartsStorageTimer_IfItemIsInStorage()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next();
        tempStorage.Add(value);
        var tempStorageItem = tempStorage[value];
        var storageTimeBeforeUpdate = tempStorageItem.Timer.Interval;

        tempStorage.TryUpdate(value, TestData.DifferentStorageTime).Should().BeTrue();

        storageTimeBeforeUpdate.Should().Be(TestData.StorageTime);
        tempStorageItem.Timer.Interval.Should().Be(TestData.DifferentStorageTime);
        tempStorageItem.Timer.ShouldBeReset();
    }

    [Fact]
    public void AddOrResetStorageTime_AddsItem_IfItemIsNotInStorage()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next();

        tempStorage.AddOrResetStorageTime(value);

        tempStorage.Contains(value).Should().BeTrue();
    }

    [Fact]
    public void AddOrResetStorageTime_ResetsCurrentStorageTimer_IfItemIsInStorage()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next(); tempStorage.Add(value);

        tempStorage.AddOrResetStorageTime(value);

        tempStorage[value].Timer.ShouldBeReset();
    }

    [Fact]
    public void AddOrUpdateStorageTime_AddsItem_WithProvidedStorageTime_IfItemIsNotInStorage()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next();

        tempStorage.AddOrUpdateStorageTime(value, TestData.DifferentStorageTime);

        tempStorage[value].Timer.Interval.Should().Be(TestData.DifferentStorageTime);
    }

    [Fact]
    public void AddOrUpdateStorageTime_UpdatesCurrentStorageTimer_IfItemIsInStorage()
    {
        var tempStorage = new TempStorage<int>(TestData.StorageTime);
        var value = new Random().Next(); tempStorage.Add(value);

        tempStorage.AddOrUpdateStorageTime(value, TestData.DifferentStorageTime);

        tempStorage[value].Timer.Interval.Should().Be(TestData.DifferentStorageTime);
    }
}

static class TestExtensions
{
    internal static void ShouldBeRemovedFrom<T>(this T item, TempStorage<T> tempStorage, Interval after)
    {
        const int extraTime = 100;
        using var tempStorageEventSource = tempStorage.Monitor();
        Thread.Sleep((int)(after + extraTime));
        tempStorageEventSource.Should().Raise(nameof(tempStorage.ItemStorageTimeElapsed))
            .WithSender(tempStorage)
            .WithArgs<TempStorageItem<T>>(tempStorageItem =>
                tempStorageItem.Value != null
                && tempStorageItem.Value.Equals(item)
                && !tempStorage.Contains(tempStorageItem.Value));
    }

    internal static void ShouldBeReset(this StorageTimer storageTimer) =>
        storageTimer.LastResetTime.Should().NotBe(storageTimer.CreationTime);
}
