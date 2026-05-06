using Project1.Collections;
using Project1.Model;
using Project1.Repository;
using TaskStatus = Project1.Model.TaskStatus;

namespace Project1.Service;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IMyCollection<TaskItem> _tasks;

    // Users worden altijd in een DoublyLinkedList bewaard (Sprint 2 vereiste).
    // De rest van de applicatie ziet alleen IMyCollection<User>.
    private readonly IMyCollection<User> _users;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        _tasks = _repository.LoadTasks();
        _users = _repository.LoadUsers();
    }

    // ── Taken ────────────────────────────────────────────────────────────────

    public IMyCollection<TaskItem> GetAllTasks() => _tasks;

    public bool AddTask(string description, TaskPriority priority)
    {
        if (string.IsNullOrWhiteSpace(description))
            return false;

        TaskItem item = new TaskItem
        {
            Id = GetNextTaskId(),
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
        TaskItem? task = FindTaskById(id);
        if (task == null) return false;

        bool removed = _tasks.Remove(task);
        if (removed) _repository.SaveTasks(_tasks);
        return removed;
    }

    public bool ToggleTaskCompletion(int id)
    {
        TaskItem? task = FindTaskById(id);
        if (task == null) return false;

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

    // ── Gebruikersbeheer (Sprint 2) ───────────────────────────────────────────

    public IMyCollection<User> GetAllUsers() => _users;

    public bool AddUser(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        User user = new User
        {
            Id = GetNextUserId(),
            Name = name.Trim()
        };

        _users.Add(user);
        _repository.SaveUsers(_users);
        return true;
    }

    public bool RemoveUser(int userId)
    {
        User? user = FindUserById(userId);
        if (user == null) return false;

        // Verwijder toewijzingen aan deze user
        IMyIterator<TaskItem> it = _tasks.GetIterator();
        while (it.HasNext())
        {
            TaskItem task = it.Next();
            if (task.AssignedUserId == userId)
                task.AssignedUserId = null;
        }

        bool removed = _users.Remove(user);
        if (removed)
        {
            _repository.SaveUsers(_users);
            _repository.SaveTasks(_tasks);
        }
        return removed;
    }

    // ── Taaktoewijzing (Sprint 2) ─────────────────────────────────────────────
    // Per-user modification rights: alleen de toegewezen user mag een taak wijzigen.
    // Dit wordt gecontroleerd via CanModify() vanuit de View.

    public bool AssignTask(int taskId, int userId)
    {
        TaskItem? task = FindTaskById(taskId);
        User? user = FindUserById(userId);
        if (task == null || user == null) return false;

        task.AssignedUserId = userId;
        _tasks.Dirty = true;
        _repository.SaveTasks(_tasks);
        return true;
    }

    public bool UnassignTask(int taskId)
    {
        TaskItem? task = FindTaskById(taskId);
        if (task == null) return false;

        task.AssignedUserId = null;
        _tasks.Dirty = true;
        _repository.SaveTasks(_tasks);
        return true;
    }

    public IMyCollection<TaskItem> GetTasksByUser(int userId)
        => _tasks.Filter(t => t.AssignedUserId == userId);

    // ── Helpers ───────────────────────────────────────────────────────────────

    private TaskItem? FindTaskById(int id)
        => _tasks.FindBy(id, (t, k) => t.Id == k);

    private User? FindUserById(int id)
        => _users.FindBy(id, (u, k) => u.Id == k);

    private int GetNextTaskId()
        => _tasks.Reduce(0, (max, t) => t.Id > max ? t.Id : max) + 1;

    private int GetNextUserId()
        => _users.Reduce(0, (max, u) => u.Id > max ? u.Id : max) + 1;
}