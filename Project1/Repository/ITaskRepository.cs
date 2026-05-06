using Project1.Collections;
using Project1.Model;

namespace Project1.Repository;

public interface ITaskRepository
{
    IMyCollection<TaskItem> LoadTasks();
    void SaveTasks(IMyCollection<TaskItem> tasks);

    // Sprint 2: users ophalen en opslaan
    IMyCollection<User> LoadUsers();
    void SaveUsers(IMyCollection<User> users);
}