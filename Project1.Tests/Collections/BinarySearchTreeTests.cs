using Xunit;
using TaskStatus = Project1.Model.TaskStatus;
using Project1.Collections;
using Project1.Model;

namespace Project1.Tests.Collections;

public class BinarySearchTreeTests
{
    private static TaskItem MakeTask(int id,
        TaskPriority priority = TaskPriority.Medium,
        TaskStatus status = TaskStatus.Todo) =>
        new TaskItem { Id = id, Description = $"Taak {id}", Priority = priority,
                       Status = status, CreatedAt = DateTime.Now };

    [Fact]
    public void Add_VerhoogtCount()
    {
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(5));
        bst.Add(MakeTask(3));
        Assert.Equal(2, bst.Count);
    }

    [Fact]
    public void ToArray_GeeftGesorteerdeVolgorde()
    {
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(5));
        bst.Add(MakeTask(3));
        bst.Add(MakeTask(7));

        var arr = bst.ToArray();
        Assert.Equal(3, arr[0].Id);
        Assert.Equal(5, arr[1].Id);
        Assert.Equal(7, arr[2].Id);
    }

    [Fact]
    public void Remove_VerwijdertItem()
    {
        var bst = new BinarySearchTree<TaskItem>();
        var t = MakeTask(5);
        bst.Add(t);
        bst.Add(MakeTask(3));
        bst.Remove(t);
        Assert.Equal(1, bst.Count);
    }

    [Fact]
    public void FindBy_VindtItem()
    {
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(1));
        bst.Add(MakeTask(2));
        bst.Add(MakeTask(3));

        var gevonden = bst.FindBy(2, (t, k) => t.Id == k);
        Assert.NotNull(gevonden);
        Assert.Equal(2, gevonden.Id);
    }

    [Fact]
    public void Filter_RetourneertJuisteItems()
    {
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(1, priority: TaskPriority.High));
        bst.Add(MakeTask(2, priority: TaskPriority.Low));
        bst.Add(MakeTask(3, priority: TaskPriority.High));

        var gefilterd = bst.Filter(t => t.Priority == TaskPriority.High);
        Assert.Equal(2, gefilterd.Count);
    }
}
