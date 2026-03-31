namespace Project1.Model;

public class TaskItem : IEquatable<TaskItem>, IComparable<TaskItem>
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool Equals(TaskItem? other)
    {
        if (other is null) return false;
        return Id == other.Id;
    }

    public int CompareTo(TaskItem? other)
    {
        if (other is null) return 1;
        return Id.CompareTo(other.Id);
    }

    public override string ToString()
    {
        string state = Completed ? "X" : " ";
        return $"{Id}. {Description} [{state}] | Priority: {Priority} | Status: {Status} | Created: {CreatedAt:yyyy-MM-dd}";
    }
}