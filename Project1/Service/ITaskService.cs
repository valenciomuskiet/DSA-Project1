using Project1.Collections;
using Project1.Model;
using TaskStatus = Project1.Model.TaskStatus;

namespace Project1.Service;

public interface ITaskService
{
    // Taken ophalen
    IMyCollection<TaskItem> GetAllTasks();

    // CRUD
    bool AddTask(string description, TaskPriority priority);
    bool UpdateTask(int id, string newDescription, TaskPriority newPriority, TaskStatus newStatus);
    bool RemoveTask(int id);
    bool ToggleTaskCompletion(int id);

    // Filteren & sorteren
    IMyCollection<TaskItem> FilterByPriority(TaskPriority priority);
    IMyCollection<TaskItem> FilterByStatus(TaskStatus status);
    IMyCollection<TaskItem> FilterByCreationDate(DateTime date);
    void SortByCreatedAtAscending();
    void SortByPriorityDescending();
}