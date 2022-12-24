namespace TempStorage.Test.TempStorage;

public class ConstructorsTest
{
    static readonly IEnumerable<int> IntCollection = Enumerable.Range(0, 10);
    const int Capacity = 10;
    static readonly EqualityComparer<AutoStorageItem<int>> TempStorageItemIntComparer = EqualityComparer<AutoStorageItem<int>>.Default;

    [Fact]
    public void Empty()
    {
        var autoStorage = new AutoStorage<int>();

        autoStorage.DefaultStorageTime.IsUnlimited.Should().BeTrue();
        autoStorage.Should().BeEmpty();
    }

    [Fact]
    public void DefaultStorageTime()
    {
        var autoStorage = new AutoStorage<int>(TestData.StorageTime);

        autoStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(autoStorage);
    }

    [Fact]
    public void DefaultStorageTimeZero_Should_SetDefaultStorageTimeToUnlimited()
    {
        var autoStorage = new AutoStorage<int>(StorageTime.Default);

        autoStorage.DefaultStorageTime.IsUnlimited.Should().BeTrue();
    }

    [Fact]
    public void Comparer_DefaultStorageTime()
    {
        var autoStorage = new AutoStorage<int>(TempStorageItemIntComparer, TestData.StorageTime);

        autoStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(autoStorage);
    }

    [Fact]
    public void Capacity_DefaultStorageTime()
    {
        var autoStorage = new AutoStorage<int>(Capacity, TestData.StorageTime);

        autoStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(autoStorage);
    }

    [Fact]
    public void Capacity_Comparer_DefaultStorageTime()
    {
        var autoStorage = new AutoStorage<int>(Capacity, TempStorageItemIntComparer, TestData.StorageTime);

        autoStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(autoStorage);
    }

    [Fact]
    public void Collection_DefaultStorageTime()
    {
        var autoStorage = new AutoStorage<int>(IntCollection, TestData.StorageTime);

        DefaultStorageTimeShouldBeSet(autoStorage);
        ShouldBeInitializedFromCollection(autoStorage);
        StorageTimersShouldBeInitialized(autoStorage);
    }

    [Fact]
    public void Collection_Comparer_DefaultStorageTime()
    {
        var autoStorage = new AutoStorage<int>(IntCollection, TempStorageItemIntComparer, TestData.StorageTime);

        DefaultStorageTimeShouldBeSet(autoStorage);
        ShouldBeInitializedFromCollection(autoStorage);
        StorageTimersShouldBeInitialized(autoStorage);
    }


    [Fact]
    public void ZeroStorageTime_ShouldNotBeSameAs_StorageTimeDefault()
    {
        var autoStorage = new AutoStorage<int>(0);

        //autoStorage.DefaultStorageTime.IsDefault
    }



    static void DefaultStorageTimeShouldBeSet<T>(AutoStorage<T> autoStorage) where T : notnull
    {
        autoStorage.DefaultStorageTime.Should().Be(TestData.StorageTime);
    }

    static void ShouldBeInitializedFromCollection(AutoStorage<int> autoStorage)
    {
        autoStorage
            .IntersectBy(IntCollection, tempStorageItem => tempStorageItem)
            .Select(tempStorageItem => tempStorageItem)
            .Should().BeEquivalentTo(IntCollection);
    }

    static void StorageTimersShouldBeInitialized<T>(AutoStorage<T> autoStorage)
    {
        foreach (var item in autoStorage)
        {
            autoStorage.TryGetStorageTimer(item, out var storageTimer);
            IsInitialized(storageTimer!).Should().BeTrue();
        }
    }

    static bool IsInitialized(StorageTimer storageTimer) => storageTimer.Interval == TestData.StorageTime && storageTimer.Enabled;
}