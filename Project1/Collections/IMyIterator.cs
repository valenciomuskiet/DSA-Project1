namespace Project1.Collections;

public interface IMyIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
}