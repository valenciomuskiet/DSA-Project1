using System.Numerics;

namespace Project1.Collections;

public class NumArray1D<T> : GenericArray<T> where T : INumber<T>, IComparable<T>, IEquatable<T>
{
    public NumArray1D(int size = 10) : base(size)
    {
    }

    public NumArray1D(T[] data) : base(data, data.Length - 1)
    {
    }

    public T? Max()
    {
        if (_index < 0)
            return default;

        T max = _data[0];
        for (int i = 1; i <= _index; i++)
        {
            if (_data[i] > max)
                max = _data[i];
        }

        return max;
    }

    public T? Min()
    {
        if (_index < 0)
            return default;

        T min = _data[0];
        for (int i = 1; i <= _index; i++)
        {
            if (_data[i] < min)
                min = _data[i];
        }

        return min;
    }

    public T? Sum()
    {
        if (_index < 0)
            return default;

        T sum = T.Zero;
        for (int i = 0; i <= _index; i++)
            sum += _data[i];

        return sum;
    }

    public T? Product(bool ignoreZeros = true)
    {
        if (_index < 0)
            return default;

        T product = T.One;
        bool found = false;

        for (int i = 0; i <= _index; i++)
        {
            if (ignoreZeros && _data[i] == T.Zero)
                continue;

            product *= _data[i];
            found = true;
        }

        return found ? product : T.Zero;
    }
}