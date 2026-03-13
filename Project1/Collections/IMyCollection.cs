namespace Project1.Collections;

public interface IMyCollection<T>
{
    int Count { get; }
    void Add(T item);
    T GetAt(int index);
    void SetAt(int index, T item);
    void RemoveAt(int index);
    T[] ToTrimmedArray();
}