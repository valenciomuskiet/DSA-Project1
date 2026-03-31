using Project1.Collections;
using Project1.Model;

namespace Project1.Service;

public interface ITaskService
{
    GenericArray<TaskItem> GetAllTasks();
    bool AddTask(string description, TaskPriority priority);
    bool UpdateTask(int id, string newDescription, TaskPriority newPriority, TaskStatus newStatus);
    bool RemoveTask(int id);
    bool ToggleTaskCompletion(int id);
    GenericArray<TaskItem> FilterByPriority(TaskPriority priority);
    GenericArray<TaskItem> FilterByStatus(TaskStatus status);
    GenericArray<TaskItem> FilterByCreationDate(DateTime date);
    void SortByCreatedAtAscending();
    void SortByPriorityDescending();
}