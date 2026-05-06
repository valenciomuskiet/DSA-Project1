namespace Project1.Collections;

/// <summary>
/// Doubly Linked List implementatie van IMyCollection.
/// Gebaseerd op slides Unit 3 (week 6-7):
///   - Node heeft Value, Next en Previous (slide 30-31)
///   - AddFirst, AddLast, InsertAfter (slides 33-35)
///   - AddSorted voor geordende invoeging (slide 36)
///   - Delete via het aanpassen van prev/next-pointers (slide 37)
///   - Search lineair van First naar Last (slide 32)
/// </summary>
public class DoublyLinkedList<T> : IMyCollection<T> where T : IEquatable<T>
{
    // ── Interne Node-klasse (slides Unit 3, slide 30) ────────────────────────
    private class Node
    {
        public T Value;
        public Node? Next;
        public Node? Previous;

        public Node(T value)
        {
            Value = value;
            Next = null;
            Previous = null;
        }
    }

    private Node? _first;
    private Node? _last;
    private int _count;

    public bool Dirty { get; set; }
    public int Count => _count;

    // ── AddFirst (slide 33) ──────────────────────────────────────────────────
    private void AddFirst(T value)
    {
        Node newNode = new Node(value);

        if (_first == null) // lege lijst
        {
            _first = newNode;
            _last = newNode;
        }
        else
        {
            newNode.Next = _first;
            _first.Previous = newNode;
            _first = newNode;
        }

        _count++;
        Dirty = true;
    }

    // ── AddLast (slide 34) ───────────────────────────────────────────────────
    private void AddLast(T value)
    {
        Node newNode = new Node(value);

        if (_first == null) // lege lijst
        {
            _first = newNode;
            _last = newNode;
        }
        else
        {
            newNode.Previous = _last;
            _last!.Next = newNode;
            _last = newNode;
        }

        _count++;
        Dirty = true;
    }

    // ── InsertAfter (slide 35) ───────────────────────────────────────────────
    private void InsertAfter(Node node, T value)
    {
        Node newNode = new Node(value);
        newNode.Next = node.Next;
        newNode.Previous = node;

        if (node.Next != null)
            node.Next.Previous = newNode;
        else
            _last = newNode; // node was Last

        node.Next = newNode;
        _count++;
        Dirty = true;
    }

    // ── Add: voegt toe aan het einde (IMyCollection vereiste) ────────────────
    public void Add(T item)
    {
        AddLast(item);
    }

    // ── Delete (slide 37) ────────────────────────────────────────────────────
    public bool Remove(T item)
    {
        Node? nodeToDelete = FindNode(item);
        if (nodeToDelete == null)
            return false;

        // Pas Previous-pointer aan
        if (nodeToDelete.Previous != null)
            nodeToDelete.Previous.Next = nodeToDelete.Next;
        else
            _first = nodeToDelete.Next; // nodeToDelete was First

        // Pas Next-pointer aan
        if (nodeToDelete.Next != null)
            nodeToDelete.Next.Previous = nodeToDelete.Previous;
        else
            _last = nodeToDelete.Previous; // nodeToDelete was Last

        _count--;
        Dirty = true;
        return true;
    }

    // ── Search (slide 32) ────────────────────────────────────────────────────
    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        Node? current = _first;
        while (current != null)
        {
            if (comparer(current.Value, key))
                return current.Value;
            current = current.Next;
        }
        return default;
    }

    // ── Filter ───────────────────────────────────────────────────────────────
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        DoublyLinkedList<T> result = new DoublyLinkedList<T>();
        Node? current = _first;
        while (current != null)
        {
            if (predicate(current.Value))
                result.Add(current.Value);
            current = current.Next;
        }
        return result;
    }

    // ── Sort: insertion sort op de linked list (Unit 2 slides) ───────────────
    // Strategie: kopieer naar array, sorteer, herbouw de lijst
    public void Sort(Comparison<T> comparison)
    {
        if (_count <= 1) return;

        T[] arr = ToArray();

        // Insertion sort (slides Unit 2)
        for (int i = 1; i < arr.Length; i++)
        {
            T key = arr[i];
            int j = i - 1;
            while (j >= 0 && comparison(arr[j], key) > 0)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }

        // Herbouw de lijst
        Clear();
        foreach (T item in arr)
            AddLast(item);

        Dirty = true;
    }

    // ── Reduce ───────────────────────────────────────────────────────────────
    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;
        Node? current = _first;
        while (current != null)
        {
            result = accumulator(result, current.Value);
            current = current.Next;
        }
        return result;
    }

    // ── Iterator ─────────────────────────────────────────────────────────────
    public IMyIterator<T> GetIterator()
    {
        return new DoublyLinkedListIterator(this);
    }

    // ── ToArray ──────────────────────────────────────────────────────────────
    public T[] ToArray()
    {
        T[] result = new T[_count];
        Node? current = _first;
        int i = 0;
        while (current != null)
        {
            result[i++] = current.Value;
            current = current.Next;
        }
        return result;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private Node? FindNode(T item)
    {
        Node? current = _first;
        while (current != null)
        {
            if (current.Value.Equals(item))
                return current;
            current = current.Next;
        }
        return null;
    }

    private void Clear()
    {
        _first = null;
        _last = null;
        _count = 0;
    }

    // ── Iterator-klasse (IMyIterator, slides Unit 3) ─────────────────────────
    private class DoublyLinkedListIterator : IMyIterator<T>
    {
        private readonly DoublyLinkedList<T> _list;
        private Node? _current;
        private bool _started;

        public DoublyLinkedListIterator(DoublyLinkedList<T> list)
        {
            _list = list;
            _current = null;
            _started = false;
        }

        public bool HasNext()
        {
            if (!_started) return _list._first != null;
            return _current?.Next != null;
        }

        public T Next()
        {
            if (!_started)
            {
                _current = _list._first;
                _started = true;
            }
            else
            {
                _current = _current?.Next;
            }
            return _current!.Value;
        }

        public void Reset()
        {
            _current = null;
            _started = false;
        }
    }
}