using Project1.Collections;
using Project1.Model;
using Project1.Service;
using TaskStatus = Project1.Model.TaskStatus;

namespace Project1.View;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _service;
    private readonly string _collectionName;

    public ConsoleTaskView(ITaskService service, string collectionName)
    {
        _service = service;
        _collectionName = collectionName;
    }

    public void Run()
    {
        while (true)
        {
            DisplayTasks(_service.GetAllTasks(), "ALLE TAKEN");
            ShowMenu();

            string option = Prompt("Kies een optie: ");

            switch (option)
            {
                case "1": AddTaskFlow();            break;
                case "2": UpdateTaskFlow();         break;
                case "3": RemoveTaskFlow();         break;
                case "4": ToggleTaskFlow();         break;
                case "5": FilterByPriorityFlow();   break;
                case "6": FilterByStatusFlow();     break;
                case "7": FilterByDateFlow();       break;
                case "8":
                    _service.SortByCreatedAtAscending();
                    Pause("Taken gesorteerd op aanmaakdatum.");
                    break;
                case "9":
                    _service.SortByPriorityDescending();
                    Pause("Taken gesorteerd op prioriteit.");
                    break;
                case "0": return;
                default:  Pause("Ongeldige optie."); break;
            }
        }
    }

    // ── Menu & display ───────────────────────────────────────────────────────

    private void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine($"  Actieve collectie: [{_collectionName}]");
        Console.WriteLine();
        Console.WriteLine("  1. Taak toevoegen");
        Console.WriteLine("  2. Taak bijwerken");
        Console.WriteLine("  3. Taak verwijderen");
        Console.WriteLine("  4. Status omschakelen");
        Console.WriteLine("  5. Filteren op prioriteit");
        Console.WriteLine("  6. Filteren op status");
        Console.WriteLine("  7. Filteren op datum");
        Console.WriteLine("  8. Sorteren op aanmaakdatum");
        Console.WriteLine("  9. Sorteren op prioriteit");
        Console.WriteLine("  0. Afsluiten");
        Console.WriteLine();
    }

    private void DisplayTasks(IMyCollection<TaskItem> tasks, string title)
    {
        Console.Clear();
        Console.WriteLine($"==== {title} ({_collectionName}) ====");
        Console.WriteLine();

        if (tasks.Count == 0)
        {
            Console.WriteLine("  Geen taken gevonden.");
            return;
        }

        IMyIterator<TaskItem> iterator = tasks.GetIterator();
        while (iterator.HasNext())
            Console.WriteLine("  " + iterator.Next());
    }

    // ── Flows ────────────────────────────────────────────────────────────────

    private void AddTaskFlow()
    {
        string description = Prompt("Omschrijving: ");
        TaskPriority priority = PromptPriority();
        bool ok = _service.AddTask(description, priority);
        Pause(ok ? "Taak toegevoegd." : "Kon taak niet toevoegen.");
    }

    private void UpdateTaskFlow()
    {
        int id = PromptInt("Taak-id om bij te werken: ");
        string description = Prompt("Nieuwe omschrijving: ");
        TaskPriority priority = PromptPriority();
        TaskStatus status = PromptStatus();
        bool ok = _service.UpdateTask(id, description, priority, status);
        Pause(ok ? "Taak bijgewerkt." : "Taak niet gevonden of ongeldige invoer.");
    }

    private void RemoveTaskFlow()
    {
        int id = PromptInt("Taak-id om te verwijderen: ");
        bool ok = _service.RemoveTask(id);
        Pause(ok ? "Taak verwijderd." : "Taak niet gevonden.");
    }

    private void ToggleTaskFlow()
    {
        int id = PromptInt("Taak-id om te togglen: ");
        bool ok = _service.ToggleTaskCompletion(id);
        Pause(ok ? "Status omgeschakeld." : "Taak niet gevonden.");
    }

    private void FilterByPriorityFlow()
    {
        TaskPriority priority = PromptPriority();
        DisplayTasks(_service.FilterByPriority(priority), $"FILTER: PRIORITEIT = {priority}");
        Pause();
    }

    private void FilterByStatusFlow()
    {
        TaskStatus status = PromptStatus();
        DisplayTasks(_service.FilterByStatus(status), $"FILTER: STATUS = {status}");
        Pause();
    }

    private void FilterByDateFlow()
    {
        string text = Prompt("Datum (yyyy-MM-dd): ");
        if (!DateTime.TryParse(text, out DateTime date))
        {
            Pause("Ongeldige datum.");
            return;
        }
        DisplayTasks(_service.FilterByCreationDate(date), $"FILTER: DATUM = {date:yyyy-MM-dd}");
        Pause();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private string Prompt(string prompt)
    {
        Console.Write("  " + prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    private int PromptInt(string prompt)
    {
        string input = Prompt(prompt);
        return int.TryParse(input, out int value) ? value : -1;
    }

    private TaskPriority PromptPriority()
    {
        Console.WriteLine("  Prioriteit: 1 = Laag, 2 = Middel, 3 = Hoog");
        string input = Prompt("Kies prioriteit: ");
        return input switch
        {
            "1" => TaskPriority.Low,
            "3" => TaskPriority.High,
            _   => TaskPriority.Medium
        };
    }

    private TaskStatus PromptStatus()
    {
        Console.WriteLine("  Status: 1 = Todo, 2 = InProgress, 3 = Done");
        string input = Prompt("Kies status: ");
        return input switch
        {
            "2" => TaskStatus.InProgress,
            "3" => TaskStatus.Done,
            _   => TaskStatus.Todo
        };
    }

    private void Pause(string? message = null)
    {
        if (!string.IsNullOrWhiteSpace(message))
            Console.WriteLine("\n  " + message);
        Console.WriteLine("  Druk op een toets om verder te gaan...");
        Console.ReadKey();
    }
}