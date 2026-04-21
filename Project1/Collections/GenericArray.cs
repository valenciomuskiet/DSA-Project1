namespace Project1.Collections;

public class GenericArray<T> : IMyCollection<T> where T : IEquatable<T>
{
    protected T[] _data;
    protected int _index;

    public bool Dirty { get; set; }

    public GenericArray(int capacity = 10)
    {
        if (capacity < 1)
            capacity = 10;

        _data = new T[capacity];
        _index = -1;
        Dirty = false;
    }

    public GenericArray(T[] data, int lastIndex)
    {
        _data = new T[data.Length];
        for (int i = 0; i < data.Length; i++)
            _data[i] = data[i];

        _index = lastIndex;
        Dirty = false;
    }

    public int Capacity => _data.Length;
    public int Count => _index + 1;
    public int LastIndex => _index;

    public T this[int index]
    {
        get
        {
            ValidateIndex(index);
            return _data[index];
        }
        set
        {
            ValidateIndex(index);
            _data[index] = value;
            Dirty = true;
        }
    }

    public void Add(T item)
    {
        Append(item);
    }

    public bool Append(T key)
    {
        EnsureCapacity();
        int next = _index + 1;
        _data[next] = key;
        _index = next;
        Dirty = true;
        return true;
    }

    public bool InsertAt(int insertIndex, T item)
    {
        if (insertIndex < 0 || insertIndex > Count)
            return false;

        EnsureCapacity();

        for (int i = _index; i >= insertIndex; i--)
            _data[i + 1] = _data[i];

        _data[insertIndex] = item;
        _index++;
        Dirty = true;
        return true;
    }

public bool AddAfter(T keyBefore, T keyToAdd)
    {
        int indexBefore = FindIndex(keyBefore);
        if (indexBefore == -1)
            return false;

        return InsertAt(indexBefore + 1, keyToAdd);
    }

    public bool Remove(T item)
    {
        return Delete(item);
    }

    public bool Delete(T key)
    {
        int index = FindIndex(key);
        if (index == -1)
            return false;

        return DeleteAt(index);
    }

    public bool DeleteAt(int index)
    {
        if (index < 0 || index > _index)
            return false;

        for (int j = index; j < _index; j++)
            _data[j] = _data[j + 1];

        _data[_index] = default!;
        _index--;
        Dirty = true;
        return true;
    }

    public int FindIndex(T item, int startIndex = 0)
    {
        for (int i = startIndex; i <= _index; i++)
        {
            if (_data[i] != null && _data[i].Equals(item))
                return i;
        }

        return -1;
    }

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        for (int i = 0; i <= _index; i++)
        {
            if (comparer(_data[i], key))
                return _data[i];
        }

        return default;
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        GenericArray<T> result = new GenericArray<T>(Count == 0 ? 1 : Count);

        for (int i = 0; i <= _index; i++)
        {
            if (predicate(_data[i]))
                result.Add(_data[i]);
        }

        result.Dirty = false;
        return result;
    }

public void Sort(Comparison<T> comparison)
    {
        Sort<T>.InsertionSort(_data, Count, comparison);
        Dirty = true;
    }

    public void Swap(int i, int j)
    {
        ValidateIndex(i);
        ValidateIndex(j);

        T temp = _data[i];
        _data[i] = _data[j];
        _data[j] = temp;
        Dirty = true;
    }

    public void Reverse()
    {
        int left = 0;
        int right = _index;

        while (left < right)
        {
            Swap(left, right);
            left++;
            right--;
        }
    }

    public T[] CloneData()
    {
        T[] copy = new T[Count];

        for (int i = 0; i < Count; i++)
            copy[i] = _data[i];

        return copy;
    }

    public void Shift(int amount, bool right = true)
    {
        if (amount <= 0 || Count == 0)
            return;

        if (right)
        {
            while (_index + amount >= _data.Length)
                Resize(_data.Length * 2);

            for (int j = _index; j >= 0; j--)
                _data[j + amount] = _data[j];

            for (int j = 0; j < amount; j++)
                _data[j] = default!;

            _index += amount;
        }
        else
        {
            if (amount > _index + 1)
            {
                Clear();
                return;
            }

            for (int j = 0; j <= _index - amount; j++)
                _data[j] = _data[j + amount];

            for (int j = _index - amount + 1; j <= _index; j++)
                _data[j] = default!;

            _index -= amount;
        }

        Dirty = true;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;

        for (int i = 0; i <= _index; i++)
            result = accumulator(result, _data[i]);

        return result;
    }

    public IMyIterator<T> GetIterator()
    {
        return new GenericArrayIterator<T>(this);
    }

    public T[] ToArray()
    {
        return CloneData();
    }

    public void Clear()
    {
        for (int i = 0; i <= _index; i++)
            _data[i] = default!;

        _index = -1;
        Dirty = true;
    }

    protected void EnsureCapacity()
    {
        if (_index + 1 < _data.Length)
            return;

        int newSize = _data.Length == 0 ? 4 : _data.Length * 2;
        Resize(newSize);
    }

    protected void Resize(int newSize)
    {
        T[] resized = new T[newSize];

        for (int i = 0; i <= _index; i++)
            resized[i] = _data[i];

        _data = resized;
    }

    protected void ValidateIndex(int index)
    {
        if (index < 0 || index > _index)
            throw new IndexOutOfRangeException($"Index {index} is outside the used range 0..{_index}.");
    }
}