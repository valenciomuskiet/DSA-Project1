using Xunit;
using TaskStatus = Project1.Model.TaskStatus;
using Project1.Collections;
using Project1.Model;

namespace Project1.Tests.Collections;

public class DoublyLinkedListTests
{
    private static TaskItem MakeTask(int id,
        TaskStatus status = TaskStatus.Todo,
        TaskPriority priority = TaskPriority.Medium) =>
        new TaskItem { Id = id, Description = $"Taak {id}", Priority = priority,
                       Status = status, CreatedAt = DateTime.Now };

    [Fact]
    public void Add_VerhoogtCount()
    {
        var lijst = new DoublyLinkedList<TaskItem>();
        lijst.Add(MakeTask(1));
        Assert.Equal(1, lijst.Count);
    }

    [Fact]
    public void Remove_BestaandItem_RetourneertTrue()
    {
        var lijst = new DoublyLinkedList<TaskItem>();
        var taak = MakeTask(1);
        lijst.Add(taak);
        bool result = lijst.Remove(taak);
        Assert.True(result);
        Assert.Equal(0, lijst.Count);
    }

    [Fact]
    public void FindBy_VindtCorrectItem()
    {
        var lijst = new DoublyLinkedList<TaskItem>();
        lijst.Add(MakeTask(1));
        lijst.Add(MakeTask(2));
        lijst.Add(MakeTask(3));

        var gevonden = lijst.FindBy(2, (t, k) => t.Id == k);
        Assert.NotNull(gevonden);
        Assert.Equal(2, gevonden.Id);
    }

    [Fact]
    public void Filter_RetourneertJuisteItems()
    {
        var lijst = new DoublyLinkedList<TaskItem>();
        lijst.Add(MakeTask(1, status: TaskStatus.Done));
        lijst.Add(MakeTask(2, status: TaskStatus.Todo));
        lijst.Add(MakeTask(3, status: TaskStatus.Done));

        var gefilterd = lijst.Filter(t => t.Status == TaskStatus.Done);
        Assert.Equal(2, gefilterd.Count);
    }

    [Fact]
    public void Sort_SorteertCorrect()
    {
        var lijst = new DoublyLinkedList<TaskItem>();
        lijst.Add(MakeTask(3));
        lijst.Add(MakeTask(1));
        lijst.Add(MakeTask(2));

        lijst.Sort((a, b) => a.Id.CompareTo(b.Id));
        var arr = lijst.ToArray();

        Assert.Equal(1, arr[0].Id);
        Assert.Equal(2, arr[1].Id);
        Assert.Equal(3, arr[2].Id);
    }
}
