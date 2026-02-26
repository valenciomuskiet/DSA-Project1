using Project1.Repository;
using Project1.Service;
using Project1.View;

namespace Project1;

class Program
{
    static void Main(string[] args)
    {
        // Dependency injection: wiring up our components
        string filePath = "tasks.json";

        ITaskRepository repository = new JsonTaskRepository(filePath);
        ITaskService service = new TaskService(repository);
        ITaskView view = new ConsoleTaskView(service);

        // Run the view
        view.Run();
    }
}