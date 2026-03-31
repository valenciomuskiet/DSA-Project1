using Project1.Collections;
using Project1.Model;
using Project1.Repository;

namespace Project1.Service;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly GenericArray<TaskItem> _tasks;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _tasks = _repository.LoadTasks();
    }

    public GenericArray<TaskItem> GetAllTasks()
    {
        return _tasks;
    }

    public bool AddTask(string description, TaskPriority priority)
    {
        if (string.IsNullOrWhiteSpace(description))
            return false;

        int newId = GetNextId();

        TaskItem item = new TaskItem
        {
            Id = newId,
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
        TaskItem? task = FindTaskById(id);
        if (task == null)
            return false;

        if (string.IsNullOrWhiteSpace(newDescription))
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
        int index = FindTaskIndexById(id);
        if (index == -1)
            return false;

        bool removed = _tasks.DeleteAt(index);
        if (removed)
            _repository.SaveTasks(_tasks);

        return removed;
    }

    public bool ToggleTaskCompletion(int id)
    {
        TaskItem? task = FindTaskById(id);
        if (task == null)
            return false;

        task.Completed = !task.Completed;
        task.Status = task.Completed ? TaskStatus.Done : TaskStatus.Todo;

        _tasks.Dirty = true;
        _repository.SaveTasks(_tasks);
        return true;
    }

    public GenericArray<TaskItem> FilterByPriority(TaskPriority priority)
    {
        return (GenericArray<TaskItem>)_tasks.Filter(t => t.Priority == priority);
    }

    public GenericArray<TaskItem> FilterByStatus(TaskStatus status)
    {
        return (GenericArray<TaskItem>)_tasks.Filter(t => t.Status == status);
    }

    public GenericArray<TaskItem> FilterByCreationDate(DateTime date)
    {
        return (GenericArray<TaskItem>)_tasks.Filter(t =>
            t.CreatedAt.Year == date.Year &&
            t.CreatedAt.Month == date.Month &&
            t.CreatedAt.Day == date.Day);
    }

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

    private int GetNextId()
    {
        int maxId = 0;
        for (int i = 0; i < _tasks.Count; i++)
        {
            if (_tasks[i].Id > maxId)
                maxId = _tasks[i].Id;
        }

        return maxId + 1;
    }

    private TaskItem? FindTaskById(int id)
    {
        return _tasks.FindBy(id, (task, key) => task.Id == key);
    }

    private int FindTaskIndexById(int id)
    {
        for (int i = 0; i < _tasks.Count; i++)
        {
            if (_tasks[i].Id == id)
                return i;
        }

        return -1;
    }
}