namespace Project1.Collections;

public interface IMyCollection<T>
{
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