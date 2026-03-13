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

    public IMyCollection<TaskItem> LoadTasks()
    {
        MyArray<TaskItem> result = new MyArray<TaskItem>();

        if (!File.Exists(_filePath))
            return result;

        string json = File.ReadAllText(_filePath);
        TaskItem[]? tasks = JsonSerializer.Deserialize<TaskItem[]>(json);

        if (tasks == null)
            return result;

        for (int i = 0; i < tasks.Length; i++)
        {
            result.Add(tasks[i]);
        }

        return result;
    }

    public void SaveTasks(IMyCollection<TaskItem> tasks)
    {
        TaskItem[] arrayToSave = tasks.ToTrimmedArray();

        string json = JsonSerializer.Serialize(arrayToSave, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
    }
}