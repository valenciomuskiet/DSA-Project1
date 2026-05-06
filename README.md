# DSA Project 1 — To-Do / Kanban

Vak: INFDSA01 | Taal: C# (.NET 10)

Console-applicatie voor taakbeheer, gebouwd zonder `System.Collections.Generic`. Alle datastructuren zijn van scratch geïmplementeerd.

---

## Projectstructuur

```
DSA-Project1-main/
├── Project1/
│   ├── Collections/    IMyCollection<T> + 4 implementaties + CollectionFactory
│   ├── Model/          TaskItem, User
│   ├── Repository/     JSON-persistentie
│   ├── Service/        Bedrijfslogica
│   ├── View/           Console-interface
│   └── Program.cs
└── Project1.Tests/
    ├── Collections/
    └── Service/
```

De lagen hangen alleen af van interfaces, nooit van concrete klassen. De `CollectionFactory` is de enige plek waar een implementatie wordt gekozen.

---

## Opstarten

```bash
cd Project1
dotnet run
```

Bij het opstarten kies je welke datastructuur gebruikt wordt:

```
╔══════════════════════════════════════════════╗
║       DSA Project 1 — Kies een collectie     ║
╠══════════════════════════════════════════════╣
║  1. Generic Array       (Sprint 1 — basis)   ║
║  2. Doubly Linked List  (Sprint 2 — users)   ║
║  3. Binary Search Tree  (Sprint 3 — Kanban)  ║
║  4. Hash Map            (Sprint 4 — lookup)  ║
╚══════════════════════════════════════════════╝
```

De actieve implementatie staat bovenaan elk scherm.

---

## Datastructuren

### Generic Array
Dynamische array, groeit met factor 2 bij capaciteitsoverschrijding.

| Operatie | Complexiteit |
|----------|-------------|
| Add | O(1) gemiddeld, O(n) bij resize |
| Remove | O(n) |
| FindBy | O(n) |
| Sort | O(n²) insertion sort |

### Doubly Linked List
Node met `Value`, `Next`, `Previous`. Gebaseerd op slides Unit 3. Wordt altijd gebruikt voor de gebruikerslijst.

| Operatie | Complexiteit |
|----------|-------------|
| AddFirst / AddLast | O(1) |
| Remove | O(n) |
| FindBy | O(n) |

### Binary Search Tree
Vergelijking via `IComparable<T>` (op `Id`). InOrder-traversal geeft gesorteerde volgorde. Gebaseerd op slides Unit 5.

| Operatie | Complexiteit |
|----------|-------------|
| Add | O(log n) gemiddeld |
| Remove | O(log n) gemiddeld |
| FindBy | O(n) voor niet-sorteersleutels |

Delete werkt in drie gevallen: geen kind, één kind, twee kinderen via in-order successor.

### Hash Map
Separate chaining met interne linked lists per bucket. Resize bij load factor > 0.7 naar volgend priemgetal. Gebaseerd op slides Unit 4.

Hashfunctie: `index = |GetHashCode()| % bucketSize`

| Operatie | Complexiteit |
|----------|-------------|
| Add / Remove / FindBy | O(1) gemiddeld |
| Filter | O(n) |

---

## Functionaliteiten per sprint

**Sprint 1** — taken toevoegen, bijwerken, verwijderen, filteren op prioriteit/status/datum, sorteren, JSON-persistentie.

**Sprint 2** — gebruikers aanmaken, taken toewijzen aan teamleden, filteren op gebruiker. Ingelogde gebruiker mag alleen eigen taken wijzigen.

**Sprint 3** — taakafhankelijkheden instellen (`CanStart` controleert of alle prereqs Done zijn), circulaire afhankelijkheden worden geblokkeerd, Kanban-weergave met drie kolommen.

**Sprint 4** — HashMap als collectie voor O(1) lookup op taak-id, demonstreerbaar via optie `h`.

---

## Tests

```bash
cd Project1.Tests
dotnet test
```

62 tests in totaal. Tests gebruiken een `InMemoryRepository` zodat er geen bestanden geschreven worden.

| Bestand | Wat wordt getest |
|---|---|
| `GenericArrayTests` | Add, Remove, FindBy, Filter, Sort, Reduce, Iterator |
| `DoublyLinkedListTests` | Zelfde + verwijderen van eerste/laatste node |
| `BinarySearchTreeTests` | InOrder volgorde, delete met twee kinderen, items behouden na sort |
| `MyHashMapTests` | Resize bij load factor, alle items na rehash vindbaar |
| `CollectionFactoryTests` | Assert.IsType per keuze |
| `TaskServiceTests` | CRUD, filteren, sprint 2 toewijzing |
| `TaskServiceDependencyTests` | CanStart, cyclus blokkeren, opruimen bij RemoveTask |

---

## IMyCollection interface

```csharp
interface IMyCollection<T> {
    void Add(T item);
    bool Remove(T item);
    T? FindBy<K>(K key, Func<T, K, bool> comparer);
    IMyCollection<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
    int Count { get; }
    bool Dirty { get; set; }
    R Reduce<R>(R initial, Func<R, T, R> accumulator);
    IMyIterator<T> GetIterator();
    T[] ToArray();
}
```

`Dirty` geeft aan of de collectie opgeslagen moet worden. `ToArray()` zorgt dat JSON-serialisatie werkt ongeacht welke implementatie actief is.
