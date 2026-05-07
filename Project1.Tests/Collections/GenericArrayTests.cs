using Xunit;
using TaskStatus = Project1.Model.TaskStatus;
using Project1.Collections;
using Project1.Model;

namespace Project1.Tests.Collections;

public class GenericArrayTests
{
    private static TaskItem MakeTask(int id, string desc = "Test",
        TaskPriority priority = TaskPriority.Medium,
        TaskStatus status = TaskStatus.Todo) =>
        new TaskItem { Id = id, Description = desc, Priority = priority,
                       Status = status, CreatedAt = DateTime.Now };

    [Fact]
    public void Add_VerhoogtCount()
    {
        var array = new GenericArray<TaskItem>();
        array.Add(MakeTask(1));
        array.Add(MakeTask(2));
        Assert.Equal(2, array.Count);
    }

    [Fact]
    public void Remove_BestaandItem_RetourneertTrue()
    {
        var array = new GenericArray<TaskItem>();
        var taak = MakeTask(1);
        array.Add(taak);
        bool result = array.Remove(taak);
        Assert.True(result);
        Assert.Equal(0, array.Count);
    }

    [Fact]
    public void Remove_NietBestaandItem_RetourneertFalse()
    {
        var array = new GenericArray<TaskItem>();
        bool result = array.Remove(MakeTask(99));
        Assert.False(result);
    }

    [Fact]
    public void FindBy_BestaandItem_Gevonden()
    {
        var array = new GenericArray<TaskItem>();
        array.Add(MakeTask(1, "Taak A"));
        array.Add(MakeTask(2, "Taak B"));

        var gevonden = array.FindBy(2, (t, k) => t.Id == k);
        Assert.NotNull(gevonden);
        Assert.Equal(2, gevonden.Id);
    }

    [Fact]
    public void Filter_RetourneertJuisteItems()
    {
        var array = new GenericArray<TaskItem>();
        array.Add(MakeTask(1, priority: TaskPriority.High));
        array.Add(MakeTask(2, priority: TaskPriority.Low));
        array.Add(MakeTask(3, priority: TaskPriority.High));

        var gefilterd = array.Filter(t => t.Priority == TaskPriority.High);
        Assert.Equal(2, gefilterd.Count);
    }

    [Fact]
    public void Sort_SorteertOpPrioriteit()
    {
        var array = new GenericArray<TaskItem>();
        array.Add(MakeTask(1, priority: TaskPriority.Low));
        array.Add(MakeTask(2, priority: TaskPriority.High));
        array.Add(MakeTask(3, priority: TaskPriority.Medium));

        array.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        var items = array.ToArray();

        Assert.Equal(TaskPriority.High, items[0].Priority);
    }
}
