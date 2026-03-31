using Project1.Repository;
using Project1.Service;
using Project1.View;

namespace Project1;

internal class Program
{
    static void Main(string[] args)
    {
        string filePath = "tasks.json";

        ITaskRepository repository = new JsonTaskRepository(filePath);
        ITaskService service = new TaskService(repository);
        ITaskView view = new ConsoleTaskView(service);

        view.Run();
    }
}