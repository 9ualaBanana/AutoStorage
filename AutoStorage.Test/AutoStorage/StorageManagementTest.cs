namespace TempStorage.Test.TempStorage;

public class StorageManagementTest
{
    [Fact]
    public void Add_AddsValue_WithDefaultStorageTime()
    {
        var defaultStorageTime = TestData.StorageTime;
        var autoStorage = new AutoStorage<int>(defaultStorageTime);
        var value = Random.Shared.Next();

        autoStorage.Add(value);

        autoStorage.Contains(value).Should().BeTrue();
        autoStorage.TryGetStorageTimer(value, out var storageTimer);
        storageTimer!.Interval.Should().Be(defaultStorageTime);
    }

    [Fact]
    public void Add_AddsValue_WithSpecifiedStorageTime()
    {
        var autoStorage = new AutoStorage<int>();
        var value = Random.Shared.Next();
        var storageTime = TestData.StorageTime;

        autoStorage.Add(value, storageTime);

        autoStorage.Contains(value).Should().BeTrue();
        autoStorage.TryGetStorageTimer(value, out var storageTimer);
        storageTimer!.Interval.Should().Be(storageTime);
    }

    [Fact]
    public void Remove_IfValueIsInStorage_ReturnsTrue()
    {
        var value = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>() { value };

        autoStorage.Remove(value).Should().BeTrue();
        autoStorage.Contains(value).Should().BeFalse();
    }

    [Fact]
    public void Remove_IfValueIsNotInStorage_ReturnsFalse()
    {
        var autoStorage = new AutoStorage<int>();

        autoStorage.Remove(Random.Shared.Next()).Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_IfValueIsInStorage_ReturnsTrue()
    {
        var value = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>() { value };

        autoStorage.TryGetValue(value, out var storedValue).Should().BeTrue();
        storedValue.Should().Be(value);
    }

    [Fact]
    public void TryGetValue_IfValueIsNotInStorage_ReturnsFalse()
    {
        var autoStorage = new AutoStorage<int>();

        autoStorage.TryGetValue(Random.Shared.Next(), out var _).Should().BeFalse();
    }
    
    [Fact]
    public void TryGetStorageTimer_IfValueIsInStorage_ReturnsTrue()
    {
        var value = Random.Shared.Next();
        var storageTime = TestData.StorageTime;
        var autoStorage = new AutoStorage<int>() { { value, storageTime } };

        autoStorage.TryGetStorageTimer(value, out var storageTimer).Should().BeTrue();
        storageTimer!.Interval.Should().Be(storageTime);
    }

    [Fact]
    public void TryGetStorageTimer_IfValueIsNotInStorage_ReturnsFalse()
    {
        var autoStorage = new AutoStorage<int>();

        autoStorage.TryGetStorageTimer(Random.Shared.Next(), out var _).Should().BeFalse();
    }

    [Fact]
    public void TryUpdateValue_IfValueIsInStorage_ReturnsTrue()
    {
        var value = Random.Shared.Next();
        var updatedValue = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>() { value };

        autoStorage.TryUpdateValue(value, updatedValue).Should().BeTrue();
        autoStorage.TryGetValue(updatedValue, out var storedValue);
        storedValue.Should().Be(updatedValue);
    }

    [Fact]
    public void TryUpdateValue_IfValueIsInStorage_ReturnsFalse()
    {
        var autoStorage = new AutoStorage<int>();

        autoStorage.TryUpdateValue(Random.Shared.Next(), Random.Shared.Next());
    }

    [Fact]
    public void TryUpdateValue_WithoutResetingStorageTimer_DoesNotResetStorageTimer()
    {
        var value = Random.Shared.Next();
        var updatedValue = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>() { { value, TestData.StorageTime } };

        autoStorage.TryUpdateValue(value, updatedValue, false).Should().BeTrue();

        autoStorage.TryGetStorageTimer(updatedValue, out var storageTimer);
        storageTimer!.LastResetTime.Should().Be(storageTimer.CreationTime);
    }

    [Fact]
    public void TryUpdateStorageTime_IfValueIsInStorage_ReturnsTrue()
    {
        var value = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>(TestData.StorageTime) { value };

        autoStorage.TryUpdateStorageTime(value, TestData.DifferentStorageTime).Should().BeTrue();

        autoStorage.TryGetStorageTimer(value, out var storageTimer);
        storageTimer!.Interval.Should().Be(TestData.DifferentStorageTime);
    }
    
    [Fact]
    public void TryUpdateStorageTime_IfValueIsNotInStorage_ReturnsFalse()
    {
        var autoStorage = new AutoStorage<int>();

        autoStorage.TryUpdateStorageTime(Random.Shared.Next(), TestData.StorageTime).Should().BeFalse();
    }

    [Fact]
    public void TryResetStorageTime_IfValueIsInStorage_ReturnsTrue()
    {
        var value = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>() { { value, TestData.StorageTime } };

        autoStorage.TryResetStorageTime(value).Should().BeTrue();

        autoStorage.TryGetStorageTimer(value, out var storageTimer);
        storageTimer!.ShouldBeReset();
    }
    
    [Fact]
    public void TryResetStorageTime_IfValueIsNotInStorage_ReturnsFalse()
    {
        var autoStorage = new AutoStorage<int>();

        autoStorage.TryResetStorageTime(Random.Shared.Next()).Should().BeFalse();
    }

    [Fact]
    public void AddOrUpdateValue_UpdatesValueAndResetsStorageTime_IfValueIsInStorage()
    {
        var value = Random.Shared.Next();
        var differentValue = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>() { value };

        autoStorage.AddOrUpdateValue(value, differentValue);

        autoStorage.Contains(value).Should().BeFalse();
        autoStorage.Contains(differentValue).Should().BeTrue();
    }

    [Fact]
    public void AddOrUpdateValue_AddsValue_WithDefaultStorageTime_IfValueIsNotInStorage()
    {
        var autoStorage = new AutoStorage<int>();
        var value = Random.Shared.Next();

        autoStorage.AddOrUpdateValue(value, Random.Shared.Next());

        autoStorage.Contains(value).Should().BeTrue();
    }

    [Fact]
    public void AddOrUpdateValue_WithoutResettingStorageTime_OnlyUpdatesValue_IfValueIsInStorage()
    {
        var value = Random.Shared.Next();
        var differentValue = Random.Shared.Next();
        var autoStorage = new AutoStorage<int>() { value };

        autoStorage.AddOrUpdateValue(value, differentValue, false);

        autoStorage.Contains(differentValue).Should().BeTrue();
        autoStorage.TryGetStorageTimer(differentValue, out var storageTimer);
        storageTimer!.ShouldNotBeReset();
    }

    [Fact]
    public void AddOrResetStorageTime_AddsItem_IfItemIsNotInStorage()
    {
        var autoStorage = new AutoStorage<int>();
        var value = new Random().Next();

        autoStorage.AddOrResetStorageTime(value);

        autoStorage.Contains(value).Should().BeTrue();
    }

    [Fact]
    public void AddOrResetStorageTime_ResetsCurrentStorageTimer_IfValueIsInStorage()
    {
        var autoStorage = new AutoStorage<int>(TestData.StorageTime);
        var value = new Random().Next(); autoStorage.Add(value);

        autoStorage.AddOrResetStorageTime(value);

        autoStorage.TryGetStorageTimer(value, out var storageTimer);
        storageTimer!.ShouldBeReset();
    }

    [Fact]
    public void AddOrUpdateStorageTime_AddsItem_WithProvidedStorageTime_IfValueIsNotInStorage()
    {
        var autoStorage = new AutoStorage<int>(TestData.StorageTime);
        var value = new Random().Next();

        autoStorage.AddOrUpdateStorageTime(value, TestData.DifferentStorageTime);

        autoStorage.TryGetStorageTimer(value, out var storageTimer);
        storageTimer!.Interval.Should().Be(TestData.DifferentStorageTime);
    }

    [Fact]
    public void AddOrUpdateStorageTime_UpdatesCurrentStorageTimer_IfItemIsInStorage()
    {
        var autoStorage = new AutoStorage<int>(TestData.StorageTime);
        var value = new Random().Next(); autoStorage.Add(value);

        autoStorage.AddOrUpdateStorageTime(value, TestData.DifferentStorageTime);

        autoStorage.TryGetStorageTimer(value, out var storageTimer);
        storageTimer!.Interval.Should().Be(TestData.DifferentStorageTime);
    }

    [Fact]
    public void AutoStorageItem_IsRemoved_WhenStorageTimeIsElapsed()
    {
        var autoStorage = new AutoStorage<int>();
        var value = new Random().Next();

        autoStorage.Add(value, TestData.ShortStorageTime);

        autoStorage.Contains(value).Should().BeTrue();
        value.ShouldBeRemovedFrom(autoStorage, after: (Interval)TestData.ShortStorageTime!);
    }
}

static class TestExtensions
{
    internal static void ShouldBeReset(this StorageTimer storageTimer) =>
        storageTimer.LastResetTime.Should().BeAfter(storageTimer.CreationTime);

    internal static void ShouldNotBeReset(this StorageTimer storageTimer) =>
        storageTimer.LastResetTime.Should().Be(storageTimer.CreationTime);

    internal static void ShouldBeRemovedFrom<T>(this T item, AutoStorage<T> tempStorage, Interval after)
    {
        const int extraTime = 100;
        using var tempStorageEventSource = tempStorage.Monitor();
        Thread.Sleep((int)(after + extraTime));
        tempStorageEventSource.Should().Raise(nameof(tempStorage.ItemStorageTimeElapsed))
            .WithSender(tempStorage)
            .WithArgs<AutoStorageItem<T>>(tempStorageItem =>
                tempStorageItem.Value != null
                && tempStorageItem.Value.Equals(item)
                && !tempStorage.Contains(tempStorageItem.Value));
    }
}
