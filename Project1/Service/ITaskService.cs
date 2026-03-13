using Project1.Collections;
using Project1.Model;

namespace Project1.Service;

public interface ITaskService
{
    IMyCollection<TaskItem> GetAllTasks();
    void AddTask(string description);
    void RemoveTask(int id);
    void ToggleTaskCompletion(int id);
}