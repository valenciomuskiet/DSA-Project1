namespace Project1.Collections;

public class MyHashMap<T> : IMyCollection<T>
    where T : IEquatable<T>, IComparable<T>
{

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
        _buckets = new ChainNode?[initialCapacity];
        _count = 0;
        Dirty = false;
    }


    private int GetBucketIndex(T item)
    {
        int hash = item.GetHashCode();
        if (hash < 0) hash = -hash; // absolute waarde
        return hash % _buckets.Length;
    }

    public void Add(T item)
    {
        double loadFactor = (double)(_count + 1) / _buckets.Length;
        if (loadFactor > LoadFactorThreshold)
            Resize();

        int index = GetBucketIndex(item);

        ChainNode newNode = new ChainNode(item);
        newNode.Next = _buckets[index];
        _buckets[index] = newNode;

        _count++;
        Dirty = true;
    }

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

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
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

    public void Sort(Comparison<T> comparison)
    {
        T[] arr = ToArray();

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

        _buckets = new ChainNode?[_buckets.Length];
        _count = 0;
        for (int i = 0; i < arr.Length; i++)
            Add(arr[i]);

        Dirty = true;
    }

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

    public IMyIterator<T> GetIterator()
    {
        return new HashMapIterator(this);
    }


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