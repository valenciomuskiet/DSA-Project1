namespace Project1.Collections;

public class GenericArray<T> : IMyCollection<T>
{
    private T[] _data;
    private int _count;

    public GenericArray(int capacity = 4)
    {
        if (capacity < 1)
        {
            capacity = 4;
        }

        _data = new T[capacity];
        _count = 0;
        Dirty = false;
    }

    public GenericArray(T[] source)
    {
        if (source == null || source.Length == 0)
        {
            _data = new T[4];
            _count = 0;
        }
        else
        {
            _data = new T[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                _data[i] = source[i];
            }

            _count = source.Length;
        }

        Dirty = false;
    }

    public int Count => _count;

    public bool Dirty { get; set; }

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
        EnsureCapacity(_count + 1);
        _data[_count] = item;
        _count++;
        Dirty = true;
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);

        if (index == -1)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        ValidateIndex(index);

        for (int i = index; i < _count - 1; i++)
        {
            _data[i] = _data[i + 1];
        }

        _data[_count - 1] = default!;
        _count--;
        Dirty = true;
    }

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        for (int i = 0; i < _count; i++)
        {
            if (comparer(_data[i], key))
            {
                return _data[i];
            }
        }

        return default;
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        GenericArray<T> result = new GenericArray<T>(_count == 0 ? 4 : _count);

        for (int i = 0; i < _count; i++)
        {
            if (predicate(_data[i]))
            {
                result.Add(_data[i]);
            }
        }

        result.Dirty = false;
        return result;
    }

    public void Sort(Comparison<T> comparison)
    {
        for (int i = 0; i < _count - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < _count; j++)
            {
                if (comparison(_data[j], _data[minIndex]) < 0)
                {
                    minIndex = j;
                }
            }

            if (minIndex != i)
            {
                T temp = _data[i];
                _data[i] = _data[minIndex];
                _data[minIndex] = temp;
            }
        }

        Dirty = true;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;

        for (int i = 0; i < _count; i++)
        {
            result = accumulator(result, _data[i]);
        }

        return result;
    }

    public IMyIterator<T> GetIterator()
    {
        return new GenericArrayIterator<T>(this);
    }

    public T[] ToArray()
    {
        T[] copy = new T[_count];

        for (int i = 0; i < _count; i++)
        {
            copy[i] = _data[i];
        }

        return copy;
    }

    private void EnsureCapacity(int neededCapacity)
    {
        if (neededCapacity <= _data.Length)
        {
            return;
        }

        int newCapacity = _data.Length * 2;

        if (newCapacity < neededCapacity)
        {
            newCapacity = neededCapacity;
        }

        T[] newData = new T[newCapacity];

        for (int i = 0; i < _count; i++)
        {
            newData[i] = _data[i];
        }

        _data = newData;
    }

    private int IndexOf(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            if (Equals(_data[i], item))
            {
                return i;
            }
        }

        return -1;
    }

    private void ValidateIndex(int index)
    {
        if (index < 0 || index >= _count)
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }
}