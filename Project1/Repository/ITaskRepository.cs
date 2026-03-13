using Project1.Collections;
using Project1.Model;

namespace Project1.Repository;

public interface ITaskRepository
{
    IMyCollection<TaskItem> LoadTasks();
    void SaveTasks(IMyCollection<TaskItem> tasks);
}