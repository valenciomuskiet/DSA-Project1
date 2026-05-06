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
        bst.Add(MakeTask(7));
        Assert.Equal(3, bst.Count);
    }

    [Fact]
    public void ToArray_GeeftGesorteerdeVolgorde()
    {
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(5));
        bst.Add(MakeTask(3));
        bst.Add(MakeTask(7));
        bst.Add(MakeTask(1));
        bst.Add(MakeTask(4));

        var arr = bst.ToArray();
        Assert.Equal(1, arr[0].Id);
        Assert.Equal(3, arr[1].Id);
        Assert.Equal(4, arr[2].Id);
        Assert.Equal(5, arr[3].Id);
        Assert.Equal(7, arr[4].Id);
    }

    [Fact]
    public void Remove_GeenKinderen_VerwijdertCorrect()
    {
        var bst = new BinarySearchTree<TaskItem>();
        var t = MakeTask(5);
        bst.Add(t);
        bool result = bst.Remove(t);
        Assert.True(result);
        Assert.Equal(0, bst.Count);
    }

    [Fact]
    public void Remove_TweeKinderen_InOrderSuccessor()
    {
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(5));
        bst.Add(MakeTask(3));
        bst.Add(MakeTask(7));

        bst.Remove(MakeTask(5));
        Assert.Equal(2, bst.Count);

        var arr = bst.ToArray();
        Assert.Equal(3, arr[0].Id);
        Assert.Equal(7, arr[1].Id);
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

    [Fact]
    public void Sort_BehouwtAlleItemsNaHerbouw()
    {
        // BST sorteert intern en herbouwt gebalanceerd.
        // ToArray() geeft altijd InOrder (op Id) terug.
        // We verifiëren dat alle items na Sort nog aanwezig zijn.
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(1, priority: TaskPriority.Low));
        bst.Add(MakeTask(2, priority: TaskPriority.High));
        bst.Add(MakeTask(3, priority: TaskPriority.Medium));

        bst.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        var arr = bst.ToArray();

        Assert.Equal(3, arr.Length);
        // Alle drie prioriteiten zijn aanwezig na sort + herbouw
        Assert.Contains(arr, t => t.Priority == TaskPriority.High);
        Assert.Contains(arr, t => t.Priority == TaskPriority.Low);
        Assert.Contains(arr, t => t.Priority == TaskPriority.Medium);
    }

    [Fact]
    public void Iterator_DoorlooptInGesorteerdeVolgorde()
    {
        var bst = new BinarySearchTree<TaskItem>();
        bst.Add(MakeTask(3));
        bst.Add(MakeTask(1));
        bst.Add(MakeTask(2));

        var ids = new List<int>();
        var it = bst.GetIterator();
        while (it.HasNext()) ids.Add(it.Next().Id);

        Assert.Equal(new[] { 1, 2, 3 }, ids);
    }
}