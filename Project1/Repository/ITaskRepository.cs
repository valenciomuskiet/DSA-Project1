using Project1.Collections;
using Project1.Model;

namespace Project1.Repository;

public interface ITaskRepository
{
    /// <summary>
    /// Laadt taken uit persistentie en geeft ze terug als de gekozen collectie.
    /// De factory bepaalt welke implementatie gebruikt wordt.
    /// </summary>
    IMyCollection<TaskItem> LoadTasks();

    /// <summary>
    /// Slaat taken op. Intern wordt ToArray() gebruikt zodat JSON-serialisatie
    /// altijd werkt, ongeacht welke collectie-implementatie actief is.
    /// </summary>
    void SaveTasks(IMyCollection<TaskItem> tasks);
}