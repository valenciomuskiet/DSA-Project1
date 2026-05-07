using Project1.Collections;
using Project1.Repository;
using Project1.Service;
using Project1.View;

namespace Project1;

internal class Program
{
    static void Main(string[] args)
    {
        // 1. Gebruiker kiest data structure
        CollectionFactory.CollectionType chosen = CollectionFactory.PromptUser();
        string collectionName = CollectionFactory.GetName(chosen);

        Console.Clear();
        Console.WriteLine($"  Gekozen: {collectionName}");
        Console.WriteLine("  Applicatie wordt gestart...");
        Console.WriteLine();

        // 2. Repository krijgt de keuze mee zodat LoadTasks() juiste collectie aanmaakt
        string filePath = "tasks.json";
        ITaskRepository repository = new JsonTaskRepository(filePath, chosen);

        // 3. Service  view zijn volledig onafhankelijk van de gekozen implementatie
        ITaskService service = new TaskService(repository);
        ITaskView view = new ConsoleTaskView(service, collectionName);

        // 4. Start applicatie
        view.Run();
    }
}