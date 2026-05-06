using Project1.Collections;
using Project1.Model;
using Project1.Repository;
using TaskStatus = Project1.Model.TaskStatus;

namespace Project1.Service;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IMyCollection<TaskItem> _tasks;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _tasks = _repository.LoadTasks();
    }

    public IMyCollection<TaskItem> GetAllTasks() => _tasks;

    public bool AddTask(string description, TaskPriority priority)
    {
        if (string.IsNullOrWhiteSpace(description))
            return false;

        TaskItem item = new TaskItem
        {
            Id = GetNextId(),
            Description = description.Trim(),
            Completed = false,
            Priority = priority,
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.Now
        };

        _tasks.Add(item);
        _repository.SaveTasks(_tasks);
        return true;
    }

    public bool UpdateTask(int id, string newDescription, TaskPriority newPriority, TaskStatus newStatus)
    {
        TaskItem? task = FindById(id);
        if (task == null || string.IsNullOrWhiteSpace(newDescription))
            return false;

        task.Description = newDescription.Trim();
        task.Priority = newPriority;
        task.Status = newStatus;
        task.Completed = newStatus == TaskStatus.Done;

        _tasks.Dirty = true;
        _repository.SaveTasks(_tasks);
        return true;
    }

    public bool RemoveTask(int id)
    {
        TaskItem? task = FindById(id);
        if (task == null)
            return false;

        bool removed = _tasks.Remove(task);
        if (removed)
            _repository.SaveTasks(_tasks);

        return removed;
    }

    public bool ToggleTaskCompletion(int id)
    {
        TaskItem? task = FindById(id);
        if (task == null)
            return false;

        task.Completed = !task.Completed;
        task.Status = task.Completed ? TaskStatus.Done : TaskStatus.Todo;

        _tasks.Dirty = true;
        _repository.SaveTasks(_tasks);
        return true;
    }

    public IMyCollection<TaskItem> FilterByPriority(TaskPriority priority)
        => _tasks.Filter(t => t.Priority == priority);

    public IMyCollection<TaskItem> FilterByStatus(TaskStatus status)
        => _tasks.Filter(t => t.Status == status);

    public IMyCollection<TaskItem> FilterByCreationDate(DateTime date)
        => _tasks.Filter(t =>
            t.CreatedAt.Year == date.Year &&
            t.CreatedAt.Month == date.Month &&
            t.CreatedAt.Day == date.Day);

    public void SortByCreatedAtAscending()
    {
        _tasks.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
        _repository.SaveTasks(_tasks);
    }

    public void SortByPriorityDescending()
    {
        _tasks.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        _repository.SaveTasks(_tasks);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private TaskItem? FindById(int id)
        => _tasks.FindBy(id, (task, key) => task.Id == key);

    private int GetNextId()
        => _tasks.Reduce(0, (max, t) => t.Id > max ? t.Id : max) + 1;
}