namespace TempStorage.Test.TempStorage;

public class ConstructorsTest
{
    static readonly IEnumerable<int> IntCollection = Enumerable.Range(0, 10);
    const int Capacity = 10;
    static readonly EqualityComparer<TempStorageItem<int>> TempStorageItemIntComparer = EqualityComparer<TempStorageItem<int>>.Default;

    [Fact]
    public void Empty()
    {
        var tempStorage = new AutoStorage<int>();

        tempStorage.DefaultStorageTime.IsUnlimited.Should().BeTrue();
        tempStorage.Should().BeEmpty();
    }

    [Fact]
    public void DefaultStorageTime()
    {
        var tempStorage = new AutoStorage<int>(TestData.StorageTime);

        tempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tempStorage);
    }

    [Fact]
    public void Comparer_DefaultStorageTime()
    {
        var tempStorage = new AutoStorage<int>(TempStorageItemIntComparer, TestData.StorageTime);

        tempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tempStorage);
    }

    [Fact]
    public void Capacity_DefaultStorageTime()
    {
        var tempStorage = new AutoStorage<int>(Capacity, TestData.StorageTime);

        tempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tempStorage);
    }

    [Fact]
    public void Capacity_Comparer_DefaultStorageTime()
    {
        var tempStorage = new AutoStorage<int>(Capacity, TempStorageItemIntComparer, TestData.StorageTime);

        tempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tempStorage);
    }

    [Fact]
    public void Collection_DefaultStorageTime()
    {
        var tempStorage = new AutoStorage<int>(IntCollection, TestData.StorageTime);

        DefaultStorageTimeShouldBeSet(tempStorage);
        ShouldBeInitializedFromCollection(tempStorage);
        StorageTimersShouldBeInitialized(tempStorage);
    }

    [Fact]
    public void Collection_Comparer_DefaultStorageTime()
    {
        var tempStorage = new AutoStorage<int>(IntCollection, TempStorageItemIntComparer, TestData.StorageTime);

        DefaultStorageTimeShouldBeSet(tempStorage);
        ShouldBeInitializedFromCollection(tempStorage);
        StorageTimersShouldBeInitialized(tempStorage);
    }



    static void DefaultStorageTimeShouldBeSet<T>(AutoStorage<T> tempStorage) where T : notnull
    {
        ((double)tempStorage.DefaultStorageTime!).Should().Be(TestData.StorageTime);
    }

    static void ShouldBeInitializedFromCollection(AutoStorage<int> tempStorage)
    {
        tempStorage
            .IntersectBy(IntCollection, tempStorageItem => tempStorageItem)
            .Select(tempStorageItem => tempStorageItem)
            .Should().BeEquivalentTo(IntCollection);
    }

    static void StorageTimersShouldBeInitialized<T>(AutoStorage<T> tempStorage) =>
        tempStorage.All(value => IsInitialized(tempStorage[value].Timer!)).Should().BeTrue();

    static bool IsInitialized(StorageTimer storageTimer) => storageTimer.Interval == TestData.StorageTime && storageTimer.Enabled;
}