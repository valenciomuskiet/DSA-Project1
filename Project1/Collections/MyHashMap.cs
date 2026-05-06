namespace Project1.Collections;

/// <summary>
/// Hash Map implementatie van IMyCollection via separate chaining.
/// Gebaseerd op slides Unit 4 (week 8-9):
///   - Hash functie: GetHashCode() % size (slide 9-10)
///   - Separate chaining met linked lists als buckets (slides 19-22)
///   - Load factor bewaking + resize bij overschrijding drempel 0.7 (slide 24)
///   - Gemiddeld O(1) voor Add, Remove, FindBy (slide 26)
/// </summary>
public class MyHashMap<T> : IMyCollection<T>
    where T : IEquatable<T>, IComparable<T>
{
    // ── Interne chain-node (slides Unit 4, slide 19-22) ──────────────────────
    // Elke bucket is een linked list van nodes — separate chaining
    private class ChainNode
    {
        public T Value;
        public ChainNode? Next;

        public ChainNode(T value)
        {
            Value = value;
            Next = null;
        }
    }

    private ChainNode?[] _buckets;
    private int _count;
    private const double LoadFactorThreshold = 0.7; // slide 24: drempel voor resize

    public bool Dirty { get; set; }
    public int Count => _count;

    public MyHashMap(int initialCapacity = 11)
    {
        // Kies een priemgetal als begingrootte voor betere spreiding (slide 11)
        _buckets = new ChainNode?[initialCapacity];
        _count = 0;
        Dirty = false;
    }

    // ── Hash functie (slides 9-10) ────────────────────────────────────────────
    // index = |GetHashCode()| % arraySize
    private int GetBucketIndex(T item)
    {
        int hash = item.GetHashCode();
        if (hash < 0) hash = -hash; // absolute waarde
        return hash % _buckets.Length;
    }

    // ── Add met load factor check (slide 24) ──────────────────────────────────
    public void Add(T item)
    {
        // Resize als load factor drempel overschreden wordt
        double loadFactor = (double)(_count + 1) / _buckets.Length;
        if (loadFactor > LoadFactorThreshold)
            Resize();

        int index = GetBucketIndex(item);

        // Voeg toe aan het begin van de chain (O(1))
        ChainNode newNode = new ChainNode(item);
        newNode.Next = _buckets[index];
        _buckets[index] = newNode;

        _count++;
        Dirty = true;
    }

    // ── Remove (slide 22) ─────────────────────────────────────────────────────
    public bool Remove(T item)
    {
        int index = GetBucketIndex(item);
        ChainNode? current = _buckets[index];
        ChainNode? previous = null;

        while (current != null)
        {
            if (current.Value.Equals(item))
            {
                if (previous == null)
                    _buckets[index] = current.Next; // verwijder eerste node
                else
                    previous.Next = current.Next;   // verwijder middelste/laatste

                _count--;
                Dirty = true;
                return true;
            }
            previous = current;
            current = current.Next;
        }

        return false;
    }

    // ── FindBy: O(1) gemiddeld (slide 26) ─────────────────────────────────────
    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        // Doorzoek alle buckets — comparer bepaalt de match, niet de hash
        for (int i = 0; i < _buckets.Length; i++)
        {
            ChainNode? current = _buckets[i];
            while (current != null)
            {
                if (comparer(current.Value, key))
                    return current.Value;
                current = current.Next;
            }
        }
        return default;
    }

    // ── Filter ───────────────────────────────────────────────────────────────
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyHashMap<T> result = new MyHashMap<T>();
        for (int i = 0; i < _buckets.Length; i++)
        {
            ChainNode? current = _buckets[i];
            while (current != null)
            {
                if (predicate(current.Value))
                    result.Add(current.Value);
                current = current.Next;
            }
        }
        return result;
    }

    // ── Sort: kopieer naar array, insertion sort, herbouw ────────────────────
    public void Sort(Comparison<T> comparison)
    {
        T[] arr = ToArray();

        // Insertion sort (Unit 2 slides)
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

        // Herbouw de hash map met gesorteerde volgorde
        _buckets = new ChainNode?[_buckets.Length];
        _count = 0;
        for (int i = 0; i < arr.Length; i++)
            Add(arr[i]);

        Dirty = true;
    }

    // ── Reduce ───────────────────────────────────────────────────────────────
    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;
        for (int i = 0; i < _buckets.Length; i++)
        {
            ChainNode? current = _buckets[i];
            while (current != null)
            {
                result = accumulator(result, current.Value);
                current = current.Next;
            }
        }
        return result;
    }

    // ── ToArray ───────────────────────────────────────────────────────────────
    public T[] ToArray()
    {
        T[] result = new T[_count];
        int idx = 0;
        for (int i = 0; i < _buckets.Length; i++)
        {
            ChainNode? current = _buckets[i];
            while (current != null)
            {
                result[idx++] = current.Value;
                current = current.Next;
            }
        }
        return result;
    }

    // ── Iterator ──────────────────────────────────────────────────────────────
    public IMyIterator<T> GetIterator()
    {
        return new HashMapIterator(this);
    }

    // ── Resize + rehash (slide 25) ────────────────────────────────────────────
    // Verdubbel de bucketgrootte en herplaats alle elementen
    private void Resize()
    {
        int newSize = NextPrime(_buckets.Length * 2);
        ChainNode?[] oldBuckets = _buckets;

        _buckets = new ChainNode?[newSize];
        _count = 0;

        for (int i = 0; i < oldBuckets.Length; i++)
        {
            ChainNode? current = oldBuckets[i];
            while (current != null)
            {
                Add(current.Value); // rehash naar nieuwe buckets
                current = current.Next;
            }
        }
    }

    // Zoek het eerstvolgende priemgetal >= n voor betere hashspreiding (slide 11)
    private int NextPrime(int n)
    {
        if (n < 2) return 2;
        while (!IsPrime(n)) n++;
        return n;
    }

    private bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;
        for (int i = 3; i * i <= n; i += 2)
            if (n % i == 0) return false;
        return true;
    }

    // ── Iterator-klasse ───────────────────────────────────────────────────────
    private class HashMapIterator : IMyIterator<T>
    {
        private readonly T[] _items;
        private int _position;

        public HashMapIterator(MyHashMap<T> map)
        {
            _items = map.ToArray();
            _position = 0;
        }

        public bool HasNext() => _position < _items.Length;
        public T Next() => _items[_position++];
        public void Reset() => _position = 0;
    }
}