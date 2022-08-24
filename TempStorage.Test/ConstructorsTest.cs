using FluentAssertions;
using System.Collections.Specialized;
using System.Timers;

namespace TempStorage.Test;

public class ConstructorsTest
{
    const double _dblStorageTime = 10_000;
    static readonly TimeSpan _tsStorageTime = TimeSpan.FromMilliseconds(_dblStorageTime);
    static readonly IEnumerable<int> _collection = Enumerable.Range(0, 10);
    const int _capacity = 10;
    static readonly EqualityComparer<TempStorageItem<int>> _comparer = EqualityComparer<TempStorageItem<int>>.Default;



    [Fact]
    public void Empty()
    {
        var tempStorage = new TempStorage<int>();

        TempStorage<int>.StorageTimeIsUnlimited(tempStorage.DefaultStorageTime.TotalMilliseconds).Should().BeTrue();
        tempStorage.Should().BeEmpty();
    }

    [Fact]
    public void DefaultStorageTime()
    {
        var tsTempStorage = new TempStorage<int>(_tsStorageTime);
        var dblTempStorage = new TempStorage<int>(_dblStorageTime);

        tsTempStorage.Should().BeEmpty();
        dblTempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tsTempStorage);
        DefaultStorageTimeShouldBeSet(dblTempStorage);
    }

    [Fact]
    public void Comparer_DefaultStorageTime()
    {
        var tsTempStorage = new TempStorage<int>(_comparer, _tsStorageTime);
        var dblTempStorage = new TempStorage<int>(_comparer, _dblStorageTime);

        tsTempStorage.Should().BeEmpty();
        dblTempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tsTempStorage);
        DefaultStorageTimeShouldBeSet(dblTempStorage);
    }

    [Fact]
    public void Capacity_DefaultStorageTime()
    {
        var tsTempStorage = new TempStorage<int>(_capacity, _tsStorageTime);
        var dblTempStorage = new TempStorage<int>(_capacity, _dblStorageTime);

        tsTempStorage.Should().BeEmpty();
        dblTempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tsTempStorage);
        DefaultStorageTimeShouldBeSet(dblTempStorage);
    }

    [Fact]
    public void Capacity_Comparer_DefaultStorageTime()
    {
        var tsTempStorage = new TempStorage<int>(_capacity, _comparer, _tsStorageTime);
        var dblTempStorage = new TempStorage<int>(_capacity, _comparer, _dblStorageTime);

        tsTempStorage.Should().BeEmpty();
        dblTempStorage.Should().BeEmpty();
        DefaultStorageTimeShouldBeSet(tsTempStorage);
        DefaultStorageTimeShouldBeSet(dblTempStorage);
    }

    [Fact]
    public void Collection_DefaultStorageTime()
    {
        var tsTempStorage = new TempStorage<int>(_collection, _tsStorageTime);
        var dblTempStorage = new TempStorage<int>(_collection, _dblStorageTime);

        DefaultStorageTimeShouldBeSet(tsTempStorage);
        DefaultStorageTimeShouldBeSet(dblTempStorage);
        ShouldBeInitializedFromCollection(tsTempStorage);
        ShouldBeInitializedFromCollection(dblTempStorage);
        StorageTimersShouldBeInitialized(tsTempStorage);
        StorageTimersShouldBeInitialized(dblTempStorage);
    }

    [Fact]
    public void Collection_Comparer_DefaultStorageTime()
    {
        var tsTempStorage = new TempStorage<int>(_collection, _comparer, _tsStorageTime);
        var dblTempStorage = new TempStorage<int>(_collection, _comparer, _dblStorageTime);

        DefaultStorageTimeShouldBeSet(tsTempStorage);
        DefaultStorageTimeShouldBeSet(dblTempStorage);
        ShouldBeInitializedFromCollection(tsTempStorage);
        ShouldBeInitializedFromCollection(dblTempStorage);
        StorageTimersShouldBeInitialized(tsTempStorage);
        StorageTimersShouldBeInitialized(dblTempStorage);
    }



    static void DefaultStorageTimeShouldBeSet<T>(TempStorage<T> tempStorage) where T : notnull
    {
        tempStorage.DefaultStorageTime.Should().Be(_tsStorageTime);
        tempStorage.DefaultStorageTime.TotalMilliseconds.Should().Be(_dblStorageTime);
    }

    static void ShouldBeInitializedFromCollection(TempStorage<int> tempStorage)
    {
        tempStorage
            .IntersectBy(_collection, tempStorageItem => tempStorageItem.Value)
            .Select(tempStorageItem => tempStorageItem.Value)
            .Should().BeEquivalentTo(_collection);
    }

    static void StorageTimersShouldBeInitialized<T>(TempStorage<T> tempStorage) =>
        tempStorage.All(tempStorageItem => IsInitialized(tempStorageItem.Timer)).Should().BeTrue();

    static bool IsInitialized(GTimer storageTimer) => storageTimer.Interval == _dblStorageTime && storageTimer.Enabled;
}