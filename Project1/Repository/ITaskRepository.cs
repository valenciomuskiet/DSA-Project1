using Project1.Collections;
using Project1.Model;

namespace Project1.Repository;

public interface ITaskRepository
{
    GenericArray<TaskItem> LoadTasks();
    void SaveTasks(GenericArray<TaskItem> tasks);
}