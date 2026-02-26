using Project1.Model;

namespace Project1.Repository;

public interface ITaskRepository {
    List<TaskItem> LoadTasks();
    void SaveTasks(List<TaskItem> tasks);
}