using Project1.Collections;
using Project1.Model;
using Project1.Repository;

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

    public IMyCollection<TaskItem> GetAllTasks()
    {
        return _tasks;
    }

    public void AddTask(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return;

        int maxId = 0;

        for (int i = 0; i < _tasks.Count; i++)
        {
            TaskItem currentTask = _tasks.GetAt(i);
            if (currentTask.Id > maxId)
            {
                maxId = currentTask.Id;
            }
        }

        TaskItem newTask = new TaskItem
        {
            Id = maxId + 1,
            Description = description,
            Completed = false
        };

        _tasks.Add(newTask);
        _repository.SaveTasks(_tasks);
    }

    public void RemoveTask(int id)
    {
        int indexToRemove = -1;

        for (int i = 0; i < _tasks.Count; i++)
        {
            if (_tasks.GetAt(i).Id == id)
            {
                indexToRemove = i;
                break;
            }
        }

        if (indexToRemove != -1)
        {
            _tasks.RemoveAt(indexToRemove);
            _repository.SaveTasks(_tasks);
        }
    }

    public void ToggleTaskCompletion(int id)
    {
        for (int i = 0; i < _tasks.Count; i++)
        {
            TaskItem task = _tasks.GetAt(i);

            if (task.Id == id)
            {
                task.Completed = !task.Completed;
                _tasks.SetAt(i, task);
                _repository.SaveTasks(_tasks);
                break;
            }
        }
    }
}