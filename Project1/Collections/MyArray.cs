namespace Project1.Collections;

public class MyArray<T> : IMyCollection<T>
{
    private T[] _items;
    public int Count { get; private set; }

    public MyArray(int capacity = 4)
    {
        _items = new T[capacity];
        Count = 0;
    }

    public void Add(T item)
    {
        if (Count == _items.Length)
        {
            Resize();
        }

        _items[Count] = item;
        Count++;
    }

    public T GetAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException();

        return _items[index];
    }

    public void SetAt(int index, T item)
    {
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException();

        _items[index] = item;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Count)
            return;

        for (int i = index; i < Count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _items[Count - 1] = default!;
        Count--;
    }

    public T[] ToTrimmedArray()
    {
        T[] result = new T[Count];

        for (int i = 0; i < Count; i++)
        {
            result[i] = _items[i];
        }

        return result;
    }

    private void Resize()
    {
        int newCapacity = _items.Length * 2;
        T[] newItems = new T[newCapacity];

        for (int i = 0; i < Count; i++)
        {
            newItems[i] = _items[i];
        }

        _items = newItems;
    }
}