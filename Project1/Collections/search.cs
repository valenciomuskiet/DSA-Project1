namespace Project1.Collections;

public static class Search<T> where T : IComparable<T>
{
    public static int SequentialSearch(T[] a, int count, T v)
    {
        if (a == null) return -1;

        for (int i = 0; i < count; i++)
        {
            if (a[i].CompareTo(v) == 0)
                return i;
        }

        return -1;
    }

    public static int BinarySearch(T[] a, int count, T v)
    {
        if (a == null) return -1;

        int low = 0;
        int high = count - 1;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;
            int cmp = a[mid].CompareTo(v);

            if (cmp == 0)
                return mid;
            if (cmp < 0)
                low = mid + 1;
            else
                high = mid - 1;
        }

        return -1;
    }
}