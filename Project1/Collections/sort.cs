namespace Project1.Collections;

public static class Sort<T>
{
    public static void SelectionSort(T[] data, int count, Comparison<T> comparison)
    {
        for (int i = 0; i < count - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < count; j++)
            {
                if (comparison(data[j], data[minIndex]) < 0)
                    minIndex = j;
            }

            if (minIndex != i)
            {
                T temp = data[i];
                data[i] = data[minIndex];
                data[minIndex] = temp;
            }
        }
    }

    public static void BubbleSort(T[] data, int count, Comparison<T> comparison)
    {
        for (int i = 0; i < count - 1; i++)
        {
            bool swapped = false;

            for (int j = 0; j < count - 1 - i; j++)
            {
                if (comparison(data[j], data[j + 1]) > 0)
                {
                    T temp = data[j];
                    data[j] = data[j + 1];
                    data[j + 1] = temp;
                    swapped = true;
                }
            }

            if (!swapped)
                break;
        }
    }

    public static void InsertionSort(T[] data, int count, Comparison<T> comparison)
    {
        for (int i = 1; i < count; i++)
        {
            T key = data[i];
            int j = i - 1;

            while (j >= 0 && comparison(data[j], key) > 0)
            {
                data[j + 1] = data[j];
                j--;
            }

            data[j + 1] = key;
        }
    }
}