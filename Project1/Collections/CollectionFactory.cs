namespace Project1.Collections;

public static class CollectionFactory
{
    public enum CollectionType
    {
        Array = 1,
        LinkedList = 2,
        BinarySearchTree = 3,
        HashMap = 4
    }

    public static CollectionType PromptUser()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║       DSA Project 1 — Kies een collectie     ║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine("║  1. Generic Array       (Sprint 1 — basis)   ║");
        Console.WriteLine("║  2. Doubly Linked List  (Sprint 2 — users)   ║");
        Console.WriteLine("║  3. Binary Search Tree  (Sprint 3 — Kanban)  ║");
        Console.WriteLine("║  4. Hash Map            (Sprint 4 — lookup)  ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();

        while (true)
        {
            Console.Write("Kies een optie (1-4): ");
            string input = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 4)
                return (CollectionType)choice;
            Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
        }
    }

    public static IMyCollection<T> Create<T>(CollectionType type) where T : IEquatable<T>
    {
        return type switch
        {
            CollectionType.Array            => new GenericArray<T>(),
            CollectionType.LinkedList       => new DoublyLinkedList<T>(),
            CollectionType.BinarySearchTree => new GenericArray<T>(), // vervangen in stap 3
            CollectionType.HashMap          => new GenericArray<T>(), // vervangen in stap 4
            _                               => new GenericArray<T>()
        };
    }

    public static string GetName(CollectionType type) => type switch
    {
        CollectionType.Array            => "Generic Array",
        CollectionType.LinkedList       => "Doubly Linked List",
        CollectionType.BinarySearchTree => "Binary Search Tree",
        CollectionType.HashMap          => "Hash Map",
        _                               => "Onbekend"
    };
}