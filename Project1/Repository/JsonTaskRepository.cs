using System.Text.Json;
using Project1.Collections;
using Project1.Model;

namespace Project1.Repository;

public class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;

    public JsonTaskRepository(string filePath)
    {
        _filePath = filePath;
    }

    public GenericArray<TaskItem> LoadTasks()
    {
        if (!File.Exists(_filePath))
            return new GenericArray<TaskItem>();

        try
        {
            string json = File.ReadAllText(_filePath);
            TaskItem[]? tasks = JsonSerializer.Deserialize<TaskItem[]>(json);

            if (tasks == null || tasks.Length == 0)
                return new GenericArray<TaskItem>();

            return new GenericArray<TaskItem>(tasks, tasks.Length - 1);
        }
        catch
        {
            return new GenericArray<TaskItem>();
        }
    }

    public void SaveTasks(GenericArray<TaskItem> tasks)
    {
        TaskItem[] snapshot = tasks.ToArray();

        string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
        tasks.Dirty = false;
    }
}