using Project1.Model;
using Project1.Repository;

namespace Project1.Service;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly List<TaskItem> _tasks;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _tasks = _repository.LoadTasks();
    }

    public IEnumerable<TaskItem> GetAllTasks() => _tasks;

    public void AddTask(string description)
    {
        int newId = _tasks.Count > 0 ? _tasks[^1].Id + 1 : 1;

        var newTask = new TaskItem
        {
            Id = newId,
            Description = description,
            Completed = false
        };

        _tasks.Add(newTask);
        _repository.SaveTasks(_tasks);
    }

    public void RemoveTask(int id)
    {
        var task = _tasks.Find(t => t.Id == id);
        if (task is null) return;

        _tasks.Remove(task);
        _repository.SaveTasks(_tasks);
    }

    public void ToggleTaskCompletion(int id)
    {
        var task = _tasks.Find(t => t.Id == id);
        if (task is null) return;

        task.Completed = !task.Completed;
        _repository.SaveTasks(_tasks);
    }
}