using System.Text.Json;
using Project1.Collections;
using Project1.Model;

namespace Project1.Repository;

public class JsonTaskRepository : ITaskRepository
{
    private readonly string _tasksPath;
    private readonly string _usersPath;
    private readonly CollectionFactory.CollectionType _collectionType;

    public JsonTaskRepository(string tasksPath, CollectionFactory.CollectionType collectionType)
    {
        _tasksPath = tasksPath;
        _usersPath = Path.Combine(
            Path.GetDirectoryName(tasksPath) ?? ".",
            "users.json");
        _collectionType = collectionType;
    }

    // ── Taken ────────────────────────────────────────────────────────────────

    public IMyCollection<TaskItem> LoadTasks()
    {
        IMyCollection<TaskItem> collection = CollectionFactory.Create<TaskItem>(_collectionType);

        if (!File.Exists(_tasksPath))
            return collection;

        try
        {
            string json = File.ReadAllText(_tasksPath);
            TaskItem[]? tasks = JsonSerializer.Deserialize<TaskItem[]>(json);
            if (tasks != null)
                foreach (TaskItem task in tasks)
                    collection.Add(task);
        }
        catch { }

        return collection;
    }

    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        string json = JsonSerializer.Serialize(tasks.ToArray(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_tasksPath, json);
        tasks.Dirty = false;
    }

    // ── Users (Sprint 2) ─────────────────────────────────────────────────────
    // Users worden altijd als DoublyLinkedList opgeslagen omdat Sprint 2
    // dit als demonstratie van de linked list data structure vereist.

    public IMyCollection<User> LoadUsers()
    {
        DoublyLinkedList<User> list = new DoublyLinkedList<User>();

        if (!File.Exists(_usersPath))
            return list;

        try
        {
            string json = File.ReadAllText(_usersPath);
            User[]? users = JsonSerializer.Deserialize<User[]>(json);
            if (users != null)
                foreach (User user in users)
                    list.Add(user);
        }
        catch { }

        return list;
    }

    public void SaveUsers(IMyCollection<User> users)
    {
        string json = JsonSerializer.Serialize(users.ToArray(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_usersPath, json);
        users.Dirty = false;
    }
}