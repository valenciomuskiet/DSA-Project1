using Xunit;
using TaskStatus = Project1.Model.TaskStatus;
using Project1.Collections;
using Project1.Model;
using Project1.Repository;
using Project1.Service;

namespace Project1.Tests.Service;

public class InMemoryRepository : ITaskRepository
{
    private IMyCollection<TaskItem> _tasks = new GenericArray<TaskItem>();
    private IMyCollection<User> _users = new GenericArray<User>();

    public IMyCollection<TaskItem> LoadTasks() => _tasks;
    public void SaveTasks(IMyCollection<TaskItem> tasks) { _tasks = tasks; }
    public IMyCollection<User> LoadUsers() => _users;
    public void SaveUsers(IMyCollection<User> users) { _users = users; }
}

public class TaskServiceTests
{
    private static TaskService MakeService() =>
        new TaskService(new InMemoryRepository());

    [Fact]
    public void AddTask_GeldigeOmschrijving_RetourneertTrue()
    {
        var service = MakeService();
        bool result = service.AddTask("Taak A", TaskPriority.High);
        Assert.True(result);
        Assert.Equal(1, service.GetAllTasks().Count);
    }

    [Fact]
    public void AddTask_LegeOmschrijving_RetourneertFalse()
    {
        var service = MakeService();
        bool result = service.AddTask("", TaskPriority.Low);
        Assert.False(result);
    }

    [Fact]
    public void RemoveTask_BestaandeId_VerwijdertTaak()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        int id = service.GetAllTasks().ToArray()[0].Id;

        bool result = service.RemoveTask(id);
        Assert.True(result);
        Assert.Equal(0, service.GetAllTasks().Count);
    }

    [Fact]
    public void ToggleTaskCompletion_WisseltStatus()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        int id = service.GetAllTasks().ToArray()[0].Id;

        service.ToggleTaskCompletion(id);
        var taak = service.GetAllTasks().FindBy(id, (t, k) => t.Id == k);

        Assert.True(taak!.Completed);
        Assert.Equal(TaskStatus.Done, taak.Status);
    }

    [Fact]
    public void FilterByPriority_RetourneertJuisteItems()
    {
        var service = MakeService();
        service.AddTask("Hoog 1", TaskPriority.High);
        service.AddTask("Laag 1", TaskPriority.Low);
        service.AddTask("Hoog 2", TaskPriority.High);

        var gefilterd = service.FilterByPriority(TaskPriority.High);
        Assert.Equal(2, gefilterd.Count);
    }

    [Fact]
    public void AssignTask_WijstToeAanUser()
    {
        var service = MakeService();
        service.AddTask("Taak A", TaskPriority.Medium);
        service.AddUser("Alice");

        int taskId = service.GetAllTasks().ToArray()[0].Id;
        int userId = service.GetAllUsers().ToArray()[0].Id;

        service.AssignTask(taskId, userId);
        var taak = service.GetAllTasks().FindBy(taskId, (t, k) => t.Id == k);
        Assert.Equal(userId, taak!.AssignedUserId);
    }
}
