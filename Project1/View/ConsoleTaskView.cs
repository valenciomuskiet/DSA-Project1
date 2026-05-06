using Project1.Collections;
using Project1.Model;
using Project1.Service;
using TaskStatus = Project1.Model.TaskStatus;

namespace Project1.View;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _service;
    private readonly string _collectionName;

    // Geselecteerde user voor per-user rechten (null = geen ingelogde user)
    private User? _currentUser;

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
                case "1":  AddTaskFlow();           break;
                case "2":  UpdateTaskFlow();        break;
                case "3":  RemoveTaskFlow();        break;
                case "4":  ToggleTaskFlow();        break;
                case "5":  FilterByPriorityFlow();  break;
                case "6":  FilterByStatusFlow();    break;
                case "7":  FilterByDateFlow();      break;
                case "8":
                    _service.SortByCreatedAtAscending();
                    Pause("Taken gesorteerd op aanmaakdatum.");
                    break;
                case "9":
                    _service.SortByPriorityDescending();
                    Pause("Taken gesorteerd op prioriteit.");
                    break;
                // Sprint 2: gebruikersbeheer
                case "u":  UserMenuFlow();          break;
                case "0":  return;
                default:   Pause("Ongeldige optie."); break;
            }
        }
    }

    // ── Menu ─────────────────────────────────────────────────────────────────

    private void ShowMenu()
    {
        string userInfo = _currentUser != null
            ? $"Ingelogd als: {_currentUser.Name}"
            : "Geen gebruiker geselecteerd";

        Console.WriteLine();
        Console.WriteLine($"  Collectie: [{_collectionName}]  |  {userInfo}");
        Console.WriteLine();
        Console.WriteLine("  1. Taak toevoegen        5. Filter op prioriteit");
        Console.WriteLine("  2. Taak bijwerken        6. Filter op status");
        Console.WriteLine("  3. Taak verwijderen      7. Filter op datum");
        Console.WriteLine("  4. Status omschakelen    8. Sorteer op datum");
        Console.WriteLine("                           9. Sorteer op prioriteit");
        Console.WriteLine("  u. Gebruikersmenu");
        Console.WriteLine("  0. Afsluiten");
        Console.WriteLine();
    }

    // ── Taken weergeven ───────────────────────────────────────────────────────

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
        {
            TaskItem task = iterator.Next();
            string userLabel = "";

            if (task.AssignedUserId.HasValue)
            {
                User? u = _service.GetAllUsers()
                    .FindBy(task.AssignedUserId.Value, (usr, id) => usr.Id == id);
                userLabel = u != null ? $" [@{u.Name}]" : " [@?]";
            }

            Console.WriteLine($"  {task}{userLabel}");
        }
    }

    // ── Taak-flows ───────────────────────────────────────────────────────────

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

        // Rechtencheck: alleen toegewezen user mag wijzigen
        if (!CanModify(id))
        {
            Pause("Geen toegang: deze taak is aan een andere gebruiker toegewezen.");
            return;
        }

        string description = Prompt("Nieuwe omschrijving: ");
        TaskPriority priority = PromptPriority();
        TaskStatus status = PromptStatus();
        bool ok = _service.UpdateTask(id, description, priority, status);
        Pause(ok ? "Taak bijgewerkt." : "Taak niet gevonden of ongeldige invoer.");
    }

    private void RemoveTaskFlow()
    {
        int id = PromptInt("Taak-id om te verwijderen: ");

        if (!CanModify(id))
        {
            Pause("Geen toegang: deze taak is aan een andere gebruiker toegewezen.");
            return;
        }

        bool ok = _service.RemoveTask(id);
        Pause(ok ? "Taak verwijderd." : "Taak niet gevonden.");
    }

    private void ToggleTaskFlow()
    {
        int id = PromptInt("Taak-id om te togglen: ");

        if (!CanModify(id))
        {
            Pause("Geen toegang: deze taak is aan een andere gebruiker toegewezen.");
            return;
        }

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

    // ── Gebruikersmenu (Sprint 2) ─────────────────────────────────────────────

    private void UserMenuFlow()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== GEBRUIKERSMENU (Doubly Linked List) ====");
            Console.WriteLine();

            IMyIterator<User> it = _service.GetAllUsers().GetIterator();
            if (!_service.GetAllUsers().Count.Equals(0))
            {
                it = _service.GetAllUsers().GetIterator();
                while (it.HasNext())
                {
                    User u = it.Next();
                    string active = _currentUser?.Id == u.Id ? " ◄ actief" : "";
                    Console.WriteLine($"  {u}{active}");
                }
            }
            else
            {
                Console.WriteLine("  Geen gebruikers.");
            }

            Console.WriteLine();
            Console.WriteLine("  1. Gebruiker toevoegen");
            Console.WriteLine("  2. Gebruiker verwijderen");
            Console.WriteLine("  3. Inloggen als gebruiker");
            Console.WriteLine("  4. Uitloggen");
            Console.WriteLine("  5. Taken van gebruiker tonen");
            Console.WriteLine("  6. Taak toewijzen");
            Console.WriteLine("  7. Toewijzing verwijderen");
            Console.WriteLine("  0. Terug");
            Console.WriteLine();

            string opt = Prompt("Kies: ");
            switch (opt)
            {
                case "1":
                    string name = Prompt("Naam: ");
                    bool added = _service.AddUser(name);
                    Pause(added ? "Gebruiker toegevoegd." : "Kon gebruiker niet toevoegen.");
                    break;

                case "2":
                    int delId = PromptInt("Gebruikers-id om te verwijderen: ");
                    bool deleted = _service.RemoveUser(delId);
                    if (deleted && _currentUser?.Id == delId)
                        _currentUser = null;
                    Pause(deleted ? "Gebruiker verwijderd." : "Niet gevonden.");
                    break;

                case "3":
                    int loginId = PromptInt("Gebruikers-id om in te loggen: ");
                    User? found = _service.GetAllUsers()
                        .FindBy(loginId, (u, k) => u.Id == k);
                    _currentUser = found;
                    Pause(found != null ? $"Ingelogd als {found.Name}." : "Gebruiker niet gevonden.");
                    break;

                case "4":
                    _currentUser = null;
                    Pause("Uitgelogd.");
                    break;

                case "5":
                    int uid = PromptInt("Gebruikers-id: ");
                    User? showUser = _service.GetAllUsers()
                        .FindBy(uid, (u, k) => u.Id == k);
                    string label = showUser != null ? $"TAKEN VAN {showUser.Name}" : "ONBEKENDE USER";
                    DisplayTasks(_service.GetTasksByUser(uid), label);
                    Pause();
                    break;

                case "6":
                    int taskId = PromptInt("Taak-id: ");
                    int userId = PromptInt("Gebruikers-id: ");
                    bool assigned = _service.AssignTask(taskId, userId);
                    Pause(assigned ? "Taak toegewezen." : "Taak of gebruiker niet gevonden.");
                    break;

                case "7":
                    int unTaskId = PromptInt("Taak-id om toewijzing te verwijderen: ");
                    bool unassigned = _service.UnassignTask(unTaskId);
                    Pause(unassigned ? "Toewijzing verwijderd." : "Taak niet gevonden.");
                    break;

                case "0":
                    return;

                default:
                    Pause("Ongeldige optie.");
                    break;
            }
        }
    }

    // ── Rechtencheck ─────────────────────────────────────────────────────────
    // Een taak mag alleen gewijzigd worden door de toegewezen user,
    // of door iemand die niet ingelogd is (onbeperkte toegang).

    private bool CanModify(int taskId)
    {
        TaskItem? task = _service.GetAllTasks()
            .FindBy(taskId, (t, k) => t.Id == k);

        if (task == null) return true;
        if (!task.AssignedUserId.HasValue) return true;
        if (_currentUser == null) return true;
        return task.AssignedUserId == _currentUser.Id;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

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