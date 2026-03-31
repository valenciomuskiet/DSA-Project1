namespace Project1.Collections;

public class GenericArrayIterator<T> : IMyIterator<T> where T : IEquatable<T>
{
    private readonly GenericArray<T> _array;
    private int _position;

    public GenericArrayIterator(GenericArray<T> array)
    {
        _array = array;
        _position = 0;
    }

    public bool HasNext()
    {
        return _position < _array.Count;
    }

    public T Next()
    {
        T value = _array[_position];
        _position++;
        return value;
    }

    public void Reset()
    {
        _position = 0;
    }
}