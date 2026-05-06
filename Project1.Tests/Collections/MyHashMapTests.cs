using Xunit;
using TaskStatus = Project1.Model.TaskStatus;
using Project1.Collections;
using Project1.Model;

namespace Project1.Tests.Collections;

public class MyHashMapTests
{
    private static TaskItem MakeTask(int id,
        TaskStatus status = TaskStatus.Todo,
        TaskPriority priority = TaskPriority.Medium) =>
        new TaskItem { Id = id, Description = $"Taak {id}", Priority = priority,
                       Status = status, CreatedAt = DateTime.Now };

    [Fact]
    public void Add_VerhoogtCount()
    {
        var map = new MyHashMap<TaskItem>();
        map.Add(MakeTask(1));
        map.Add(MakeTask(2));
        Assert.Equal(2, map.Count);
    }

    [Fact]
    public void Remove_BestaandItem_RetourneertTrue()
    {
        var map = new MyHashMap<TaskItem>();
        var taak = MakeTask(1);
        map.Add(taak);
        bool result = map.Remove(taak);
        Assert.True(result);
        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void Remove_NietBestaand_RetourneertFalse()
    {
        var map = new MyHashMap<TaskItem>();
        bool result = map.Remove(MakeTask(99));
        Assert.False(result);
    }

    [Fact]
    public void FindBy_VindtItem()
    {
        var map = new MyHashMap<TaskItem>();
        map.Add(MakeTask(1));
        map.Add(MakeTask(2));
        map.Add(MakeTask(3));

        var gevonden = map.FindBy(2, (t, k) => t.Id == k);
        Assert.NotNull(gevonden);
        Assert.Equal(2, gevonden.Id);
    }

    [Fact]
    public void LoadFactor_TriggertResize()
    {
        var map = new MyHashMap<TaskItem>(11);
        for (int i = 1; i <= 10; i++)
            map.Add(MakeTask(i));

        Assert.Equal(10, map.Count);
        for (int i = 1; i <= 10; i++)
            Assert.NotNull(map.FindBy(i, (t, k) => t.Id == k));
    }

    [Fact]
    public void Filter_RetourneertJuisteItems()
    {
        var map = new MyHashMap<TaskItem>();
        map.Add(MakeTask(1, status: TaskStatus.Done));
        map.Add(MakeTask(2, status: TaskStatus.Todo));
        map.Add(MakeTask(3, status: TaskStatus.Done));

        var gefilterd = map.Filter(t => t.Status == TaskStatus.Done);
        Assert.Equal(2, gefilterd.Count);
    }

    [Fact]
    public void Reduce_TeltItems()
    {
        var map = new MyHashMap<TaskItem>();
        map.Add(MakeTask(1));
        map.Add(MakeTask(2));
        map.Add(MakeTask(3));

        int totaal = map.Reduce(0, (acc, _) => acc + 1);
        Assert.Equal(3, totaal);
    }

    [Fact]
    public void Iterator_DoorlooptAlleItems()
    {
        var map = new MyHashMap<TaskItem>();
        map.Add(MakeTask(1));
        map.Add(MakeTask(2));
        map.Add(MakeTask(3));

        int count = 0;
        var it = map.GetIterator();
        while (it.HasNext()) { it.Next(); count++; }
        Assert.Equal(3, count);
    }
}