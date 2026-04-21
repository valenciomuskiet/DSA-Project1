using Project1.Collections;
using Project1.Model;
using Project1.Service;
using TaskStatus = Project1.Model.TaskStatus;


namespace Project1.View;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _service;

    public ConsoleTaskView(ITaskService service)
    {
        _service = service;
    }

    public void Run()
    {
        while (true)
        {
            DisplayTasks(_service.GetAllTasks(), "ALL TASKS");
            ShowMenu();

            string option = Prompt("Select an option: ");

            switch (option)
            {
                case "1":
                    AddTaskFlow();
                    break;
                case "2":
                    UpdateTaskFlow();
                    break;
                case "3":
                    RemoveTaskFlow();
                    break;
                case "4":
                    ToggleTaskFlow();
                    break;
                case "5":
                    FilterByPriorityFlow();
                    break;
                case "6":
                    FilterByStatusFlow();
                    break;
                case "7":
                    FilterByDateFlow();
                    break;
                case "8":
                    _service.SortByCreatedAtAscending();
                    Pause("Tasks sorted by creation date.");
                    break;
                case "9":
                    _service.SortByPriorityDescending();
                    Pause("Tasks sorted by priority.");
                    break;
                case "0":
                    return;
                default:
                    Pause("Invalid option.");
                    break;
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("1. Add Task");
        Console.WriteLine("2. Update Task");
        Console.WriteLine("3. Remove Task");
        Console.WriteLine("4. Toggle Task State");
        Console.WriteLine("5. Filter by Priority");
        Console.WriteLine("6. Filter by Status");
        Console.WriteLine("7. Filter by Creation Date");
        Console.WriteLine("8. Sort by Creation Date");
        Console.WriteLine("9. Sort by Priority");
        Console.WriteLine("0. Exit");
        Console.WriteLine();
    }



    private void DisplayTasks(GenericArray<TaskItem> tasks, string title)
    {
        Console.Clear();
        Console.WriteLine($"==== {title} ====");

        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        IMyIterator<TaskItem> iterator = tasks.GetIterator();
        while (iterator.HasNext())
        {
            TaskItem task = iterator.Next();
            Console.WriteLine(task);
        }
    }

    private void AddTaskFlow()
    {
        string description = Prompt("Enter task description: ");
        TaskPriority priority = PromptPriority();

        bool success = _service.AddTask(description, priority);
        Pause(success ? "Task added." : "Task could not be added.");
    }

    private void UpdateTaskFlow()
    {
        int id = PromptInt("Enter task id to update: ");
        string description = Prompt("Enter new description: ");
        TaskPriority priority = PromptPriority();
        TaskStatus status = PromptStatus();

        bool success = _service.UpdateTask(id, description, priority, status);
        Pause(success ? "Task updated." : "Task not found or invalid input.");
    }

    private void RemoveTaskFlow()
    {
        int id = PromptInt("Enter task id to remove: ");
        bool success = _service.RemoveTask(id);
        Pause(success ? "Task removed." : "Task not found.");
    }

    private void ToggleTaskFlow()
    {
        int id = PromptInt("Enter task id to toggle: ");
        bool success = _service.ToggleTaskCompletion(id);
        Pause(success ? "Task toggled." : "Task not found.");
    }

    private void FilterByPriorityFlow()
    {
        TaskPriority priority = PromptPriority();
        GenericArray<TaskItem> filtered = _service.FilterByPriority(priority);
        DisplayTasks(filtered, $"FILTER: PRIORITY = {priority}");
        Pause();
    }

    private void FilterByStatusFlow()
    {
        TaskStatus status = PromptStatus();
        GenericArray<TaskItem> filtered = _service.FilterByStatus(status);
        DisplayTasks(filtered, $"FILTER: STATUS = {status}");
        Pause();
    }

    private void FilterByDateFlow()
    {
        string text = Prompt("Enter date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(text, out DateTime date))
        {
            Pause("Invalid date.");
            return;
        }

        GenericArray<TaskItem> filtered = _service.FilterByCreationDate(date);
        DisplayTasks(filtered, $"FILTER: DATE = {date:yyyy-MM-dd}");
        Pause();
    }

    private string Prompt(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    private int PromptInt(string prompt)
    {
        string input = Prompt(prompt);
        return int.TryParse(input, out int value) ? value : -1;
    }

    private TaskPriority PromptPriority()
    {
        Console.WriteLine("Priority: 1 = Low, 2 = Medium, 3 = High");
        string input = Prompt("Choose priority: ");

        return input switch
        {
            "1" => TaskPriority.Low,
            "2" => TaskPriority.Medium,
            "3" => TaskPriority.High,
            _ => TaskPriority.Medium
        };
    }

    private TaskStatus PromptStatus()
    {
        Console.WriteLine("Status: 1 = Todo, 2 = InProgress, 3 = Done");
        string input = Prompt("Choose status: ");

        return input switch
        {
            "1" => TaskStatus.Todo,
            "2" => TaskStatus.InProgress,
            "3" => TaskStatus.Done,
            _ => TaskStatus.Todo
        };
    }

    private void Pause(string? message = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
            Console.WriteLine(message);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}