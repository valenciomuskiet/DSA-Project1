using Project1.Collections;
using Project1.Model;
using Project1.Service;
using TaskStatus = Project1.Model.TaskStatus;

namespace Project1.View;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _service;
    private readonly string _collectionName;
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
                case "k":  KanbanFlow();            break;
                case "d":  DependencyMenuFlow();    break;
                case "u":  UserMenuFlow();          break;
                case "h":  HashLookupFlow();        break;
                case "0":  return;
                default:   Pause("Ongeldige optie."); break;
            }
        }
    }

    //  Menu

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
        Console.WriteLine("  k. Kanban-weergave");
        Console.WriteLine("  d. Afhankelijkhedenmenu");
        Console.WriteLine("  u. Gebruikersmenu");
        Console.WriteLine("  h. HashMap lookup (Sprint 4)");
        Console.WriteLine("  0. Afsluiten");
        Console.WriteLine();
    }

    // Taken weergeven 

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
            Console.WriteLine("  " + FormatTask(iterator.Next()));
    }

    private string FormatTask(TaskItem task)
    {
        string userLabel = "";
        if (task.AssignedUserId.HasValue)
        {
            User? u = _service.GetAllUsers()
                .FindBy(task.AssignedUserId.Value, (usr, id) => usr.Id == id);
            userLabel = u != null ? $" [@{u.Name}]" : " [@?]";
        }

        string blocked = !CanStart(task) ? " [GEBLOKKEERD]" : "";
        return $"{task}{userLabel}{blocked}";
    }

    // Kanban-weergave 

    private void KanbanFlow()
    {
        Console.Clear();
        Console.WriteLine($"==== KANBAN ({_collectionName}) ====");
        Console.WriteLine();

        IMyCollection<TaskItem> todo       = _service.FilterByStatus(TaskStatus.Todo);
        IMyCollection<TaskItem> inProgress = _service.FilterByStatus(TaskStatus.InProgress);
        IMyCollection<TaskItem> done       = _service.FilterByStatus(TaskStatus.Done);

        int colWidth = 26;
        string sep = new string('─', colWidth);

        Console.WriteLine("  " + "TODO".PadRight(colWidth) + "  " + "IN PROGRESS".PadRight(colWidth) + "  " + "DONE");
        Console.WriteLine($"  {sep}  {sep}  {sep}");

        // Zet elke kolom om naar array voor rij-gewijze weergave
        TaskItem[] todoArr       = todo.ToArray();
        TaskItem[] inProgressArr = inProgress.ToArray();
        TaskItem[] doneArr       = done.ToArray();

        int maxRows = Math.Max(todoArr.Length, Math.Max(inProgressArr.Length, doneArr.Length));

        for (int i = 0; i < maxRows; i++)
        {
            string col1 = i < todoArr.Length
                ? TruncateKanban(todoArr[i], colWidth) : "";
            string col2 = i < inProgressArr.Length
                ? TruncateKanban(inProgressArr[i], colWidth) : "";
            string col3 = i < doneArr.Length
                ? TruncateKanban(doneArr[i], colWidth) : "";

            Console.WriteLine("  " + col1.PadRight(colWidth) + "  " + col2.PadRight(colWidth) + "  " + col3);
        }

        Console.WriteLine();
        Console.WriteLine($"  Todo: {todoArr.Length}  |  In Progress: {inProgressArr.Length}  |  Done: {doneArr.Length}");
        Console.WriteLine();

        // Toon geblokkeerde taken apart
        IMyCollection<TaskItem> blocked = _service.GetBlockedTasks();
        if (blocked.Count > 0)
        {
            Console.WriteLine("  Geblokkeerde taken (wachten op prereqs):");
            IMyIterator<TaskItem> it = blocked.GetIterator();
            while (it.HasNext())
            {
                TaskItem t = it.Next();
                string prereqs = string.Join(", ", t.DependsOn);
                Console.WriteLine($"    #{t.Id} {t.Description} — wacht op: {prereqs}");
            }
        }

        Pause();
    }

    private string TruncateKanban(TaskItem task, int width)
    {
        string blocked = !CanStart(task) ? "!" : " ";
        string line = $"{blocked}#{task.Id} {task.Description}";
        return line.Length > width ? line[..(width - 1)] + "…" : line;
    }

    private bool CanStart(TaskItem task)
        => _service.CanStart(task.Id);

    // Afhankelijkhedenmenu

    private void DependencyMenuFlow()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== AFHANKELIJKHEDEN (BST) ====");
            Console.WriteLine();

            // Toon alle taken met hun prereqs
            IMyIterator<TaskItem> it = _service.GetAllTasks().GetIterator();
            while (it.HasNext())
            {
                TaskItem t = it.Next();
                if (t.DependsOn.Length > 0)
                {
                    bool kanStarten = _service.CanStart(t.Id);
                    string status = kanStarten ? "✓ kan starten" : "✗ geblokkeerd";
                    Console.WriteLine($"  #{t.Id} {t.Description}");
                    Console.WriteLine($"      wacht op: [{string.Join(", ", t.DependsOn)}] — {status}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  1. Afhankelijkheid toevoegen");
            Console.WriteLine("  2. Afhankelijkheid verwijderen");
            Console.WriteLine("  3. Controleer of taak kan starten");
            Console.WriteLine("  0. Terug");
            Console.WriteLine();

            string opt = Prompt("Kies: ");
            switch (opt)
            {
                case "1":
                    int taskId  = PromptInt("Taak-id: ");
                    int prereqId = PromptInt("Vereiste taak-id: ");
                    bool added = _service.AddDependency(taskId, prereqId);
                    Pause(added ? "Afhankelijkheid toegevoegd." :
                        "Mislukt (niet gevonden, zelfde taak of cyclus).");
                    break;

                case "2":
                    int rmTask   = PromptInt("Taak-id: ");
                    int rmPrereq = PromptInt("Vereiste taak-id om te verwijderen: ");
                    bool removed = _service.RemoveDependency(rmTask, rmPrereq);
                    Pause(removed ? "Afhankelijkheid verwijderd." : "Niet gevonden.");
                    break;

                case "3":
                    int checkId = PromptInt("Taak-id om te controleren: ");
                    bool can = _service.CanStart(checkId);
                    Pause(can ? "Taak kan starten (alle prereqs zijn Done)."
                              : "Taak is geblokkeerd (niet alle prereqs zijn Done).");
                    break;

                case "0": return;
                default: Pause("Ongeldige optie."); break;
            }
        }
    }

    // Taak-flows

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
        if (!CanModify(id))
        {
            Pause("Geen toegang: taak is aan een andere gebruiker toegewezen.");
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
            Pause("Geen toegang: taak is aan een andere gebruiker toegewezen.");
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
            Pause("Geen toegang: taak is aan een andere gebruiker toegewezen.");
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

    // HashMap lookup 
    // Demonstreert O(1) gemiddelde opzoektijd via taak-id.
    // Toont ook de load factor en bucket-info zodat de HashMap zichtbaar is.

    private void HashLookupFlow()
    {
        Console.Clear();
        Console.WriteLine("==== HASHMAP LOOKUP (Sprint 4) ====");
        Console.WriteLine();
        Console.WriteLine("  De HashMap slaat taken op in buckets via GetHashCode() % size.");
        Console.WriteLine("  Opzoeken op id is gemiddeld O(1) — ongeacht het aantal taken.");
        Console.WriteLine();

        int id = PromptInt("Zoek taak op id: ");

        // Meting: zoek via de actieve collectie (werkt altijd via FindBy)
        TaskItem? task = _service.GetAllTasks().FindBy(id, (t, k) => t.Id == k);

        if (task != null)
        {
            Console.WriteLine();
            Console.WriteLine("  Gevonden:");
            Console.WriteLine("  " + FormatTask(task));
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"  Geen taak gevonden met id {id}.");
        }

        Console.WriteLine();
        Console.WriteLine($"  Totaal taken in collectie: {_service.GetAllTasks().Count}");
        Console.WriteLine($"  Actieve collectie: {_collectionName}");
        Pause();
    }

    // Gebruikersmenu 

    private void UserMenuFlow()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== GEBRUIKERSMENU (Doubly Linked List) ====");
            Console.WriteLine();

            IMyCollection<User> users = _service.GetAllUsers();
            if (users.Count == 0)
            {
                Console.WriteLine("  Geen gebruikers.");
            }
            else
            {
                IMyIterator<User> it = users.GetIterator();
                while (it.HasNext())
                {
                    User u = it.Next();
                    string active = _currentUser?.Id == u.Id ? " ◄ actief" : "";
                    Console.WriteLine($"  {u}{active}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  1. Gebruiker toevoegen    5. Taken van gebruiker");
            Console.WriteLine("  2. Gebruiker verwijderen  6. Taak toewijzen");
            Console.WriteLine("  3. Inloggen               7. Toewijzing verwijderen");
            Console.WriteLine("  4. Uitloggen");
            Console.WriteLine("  0. Terug");
            Console.WriteLine();

            string opt = Prompt("Kies: ");
            switch (opt)
            {
                case "1":
                    string name = Prompt("Naam: ");
                    Pause(_service.AddUser(name) ? "Gebruiker toegevoegd." : "Mislukt.");
                    break;
                case "2":
                    int delId = PromptInt("Gebruikers-id: ");
                    bool del = _service.RemoveUser(delId);
                    if (del && _currentUser?.Id == delId) _currentUser = null;
                    Pause(del ? "Verwijderd." : "Niet gevonden.");
                    break;
                case "3":
                    int loginId = PromptInt("Gebruikers-id: ");
                    _currentUser = _service.GetAllUsers().FindBy(loginId, (u, k) => u.Id == k);
                    Pause(_currentUser != null ? $"Ingelogd als {_currentUser.Name}." : "Niet gevonden.");
                    break;
                case "4":
                    _currentUser = null;
                    Pause("Uitgelogd.");
                    break;
                case "5":
                    int uid = PromptInt("Gebruikers-id: ");
                    User? showUser = _service.GetAllUsers().FindBy(uid, (u, k) => u.Id == k);
                    DisplayTasks(_service.GetTasksByUser(uid),
                        showUser != null ? $"TAKEN VAN {showUser.Name}" : "ONBEKENDE USER");
                    Pause();
                    break;
                case "6":
                    int tId = PromptInt("Taak-id: ");
                    int uId = PromptInt("Gebruikers-id: ");
                    Pause(_service.AssignTask(tId, uId) ? "Toegewezen." : "Niet gevonden.");
                    break;
                case "7":
                    int unId = PromptInt("Taak-id: ");
                    Pause(_service.UnassignTask(unId) ? "Toewijzing verwijderd." : "Niet gevonden.");
                    break;
                case "0": return;
                default: Pause("Ongeldige optie."); break;
            }
        }
    }

    //  Rechtencheck 

    private bool CanModify(int taskId)
    {
        TaskItem? task = _service.GetAllTasks().FindBy(taskId, (t, k) => t.Id == k);
        if (task == null || !task.AssignedUserId.HasValue) return true;
        if (_currentUser == null) return true;
        return task.AssignedUserId == _currentUser.Id;
    }

    // Helpers

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