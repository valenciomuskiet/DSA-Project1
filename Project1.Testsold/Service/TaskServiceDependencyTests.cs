using Xunit;
using TaskStatus = Project1.Model.TaskStatus;
using Project1.Collections;
using Project1.Model;
using Project1.Repository;
using Project1.Service;

namespace Project1.Tests.Service;

public class TaskServiceDependencyTests
{
    private static TaskService MakeService() =>
        new TaskService(new InMemoryRepository());

    [Fact]
    public void AddDependency_GeldigePair_VoegtToe()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddTask("Taak B", TaskPriority.Medium);

        var taken = service.GetAllTasks().ToArray();
        int idA = taken[0].Id;
        int idB = taken[1].Id;

        bool result = service.AddDependency(idB, idA);
        Assert.True(result);

        var taakB = service.GetAllTasks().FindBy(idB, (t, k) => t.Id == k);
        Assert.Contains(idA, taakB!.DependsOn);
    }

    [Fact]
    public void CanStart_ZonderPrereqs_RetourneertTrue()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        int id = service.GetAllTasks().ToArray()[0].Id;
        Assert.True(service.CanStart(id));
    }

    [Fact]
    public void CanStart_PrereqNietDone_RetourneertFalse()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddTask("Taak B", TaskPriority.Medium);

        var taken = service.GetAllTasks().ToArray();
        int idA = taken[0].Id;
        int idB = taken[1].Id;

        service.AddDependency(idB, idA);
        Assert.False(service.CanStart(idB));
    }

    [Fact]
    public void CanStart_PrereqIsDone_RetourneertTrue()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddTask("Taak B", TaskPriority.Medium);

        var taken = service.GetAllTasks().ToArray();
        int idA = taken[0].Id;
        int idB = taken[1].Id;

        service.AddDependency(idB, idA);
        service.ToggleTaskCompletion(idA);

        Assert.True(service.CanStart(idB));
    }

    [Fact]
    public void AddDependency_CirkulaireAfhankelijkheid_WordtGeweigerd()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddTask("Taak B", TaskPriority.Medium);

        var taken = service.GetAllTasks().ToArray();
        int idA = taken[0].Id;
        int idB = taken[1].Id;

        service.AddDependency(idB, idA);
        bool result = service.AddDependency(idA, idB);
        Assert.False(result);
    }

    [Fact]
    public void AddDependency_ZelfdeTask_WordtGeweigerd()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        int id = service.GetAllTasks().ToArray()[0].Id;

        bool result = service.AddDependency(id, id);
        Assert.False(result);
    }

    [Fact]
    public void RemoveDependency_VerwijdertPrereq()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddTask("Taak B", TaskPriority.Medium);

        var taken = service.GetAllTasks().ToArray();
        int idA = taken[0].Id;
        int idB = taken[1].Id;

        service.AddDependency(idB, idA);
        service.RemoveDependency(idB, idA);

        Assert.True(service.CanStart(idB));
    }

    [Fact]
    public void GetBlockedTasks_RetourneertGeblokkeerde()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddTask("Taak B", TaskPriority.Medium);

        var taken = service.GetAllTasks().ToArray();
        int idA = taken[0].Id;
        int idB = taken[1].Id;

        service.AddDependency(idB, idA);

        var geblokkeerd = service.GetBlockedTasks();
        Assert.Equal(1, geblokkeerd.Count);
    }

    [Fact]
    public void RemoveTask_VerwijdertOokAlsPrereq()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddTask("Taak B", TaskPriority.Medium);

        var taken = service.GetAllTasks().ToArray();
        int idA = taken[0].Id;
        int idB = taken[1].Id;

        service.AddDependency(idB, idA);
        service.RemoveTask(idA);

        var taakB = service.GetAllTasks().FindBy(idB, (t, k) => t.Id == k);
        Assert.Empty(taakB!.DependsOn);
    }
}