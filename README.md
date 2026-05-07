# DSA Project 1 — Task manager

INFDSA01 | C# (.NET 10)

Console app voor taakbeheer. Geen System.Collections.Generic gebruikt, alles zelf geïmplementeerd.

## Opstarten

Zorg dat .NET 10 geïnstalleerd is.

```bash
cd Project1
dotnet run
```

Bij het opstarten kies je welke datastructuur je wilt gebruiken:

```
1. Generic Array
2. Doubly Linked List
3. Binary Search Tree
4. Hash Map
```

## Datastructuren

**Generic Array** — dynamische array die groeit als hij vol is (factor 2).

**Doubly Linked List** — nodes met prev/next pointers. Wordt ook gebruikt voor de gebruikerslijst.

**Binary Search Tree** — sorteert op Id. InOrder traversal geeft gesorteerde volgorde. Delete werkt via in-order successor als een node twee kinderen heeft.

**Hash Map** — separate chaining. Resize zodra load factor boven de 0.7 komt, nieuwe grootte is een priemgetal.

## Tests uitvoeren

```bash
cd Project1.Tests
dotnet test
```

Voeg het testproject eerst toe als het nog niet in de solution zit:

```bash
dotnet sln add Project1.Tests/Project1.Tests.csproj
dotnet restore
```
