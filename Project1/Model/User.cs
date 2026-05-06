namespace Project1.Model;

public class User : IEquatable<User>, IComparable<User>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public bool Equals(User? other)
    {
        if (other is null) return false;
        return Id == other.Id;
    }

    public int CompareTo(User? other)
    {
        if (other is null) return 1;
        return Id.CompareTo(other.Id);
    }

    public override string ToString() => $"[{Id}] {Name}";
}