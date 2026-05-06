using System.Text.Json;
using Project1.Collections;
using Project1.Model;

namespace Project1.Repository;

public class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    private readonly CollectionFactory.CollectionType _collectionType;

    public JsonTaskRepository(string filePath, CollectionFactory.CollectionType collectionType)
    {
        _filePath = filePath;
        _collectionType = collectionType;
    }

    public IMyCollection<TaskItem> LoadTasks()
    {
        IMyCollection<TaskItem> collection = CollectionFactory.Create<TaskItem>(_collectionType);

        if (!File.Exists(_filePath))
            return collection;

        try
        {
            string json = File.ReadAllText(_filePath);
            TaskItem[]? tasks = JsonSerializer.Deserialize<TaskItem[]>(json);

            if (tasks == null)
                return collection;

            foreach (TaskItem task in tasks)
                collection.Add(task);
        }
        catch
        {
            // Bij corrupt bestand: lege collectie teruggeven
        }

        return collection;
    }

    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        // ToArray() werkt op elke IMyCollection-implementatie.
        // JSON-serialisatie ziet altijd een gewone array, ongeacht de interne structuur.
        TaskItem[] snapshot = tasks.ToArray();

        string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
        tasks.Dirty = false;
    }
}