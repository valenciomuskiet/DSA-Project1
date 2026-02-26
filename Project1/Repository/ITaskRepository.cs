namespace Project1.Model;


interface ITaskRepository {
    List<TaskItem> LoadTasks();
    void SaveTasks(List<TaskItem> tasks);
}