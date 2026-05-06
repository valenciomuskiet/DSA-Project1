namespace Project1.Model;

/// <summary>
/// Stelt een teamlid voor aan wie taken kunnen worden toegewezen.
/// Opgeslagen als DoublyLinkedList van Users (Sprint 2).
/// </summary>
public class User : IEquatable<User>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public bool Equals(User? other)
    {
        if (other is null) return false;
        return Id == other.Id;
    }

    public override string ToString() => $"[{Id}] {Name}";
}