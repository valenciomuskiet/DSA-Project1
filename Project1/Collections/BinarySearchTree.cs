namespace Project1.Collections;
public class BinarySearchTree<T> : IMyCollection<T>
    where T : IEquatable<T>, IComparable<T>
{
    private class Node
    {
        public T Value;
        public Node? Left;
        public Node? Right;

        public Node(T value)
        {
            Value = value;
            Left = null;
            Right = null;
        }
    }

    private Node? _root;
    private int _count;

    public bool Dirty { get; set; }
    public int Count => _count;

    public void Add(T item)
    {
        _root = Insert(_root, item);
        _count++;
        Dirty = true;
    }

    private Node Insert(Node? node, T item)
    {
        if (node == null)
            return new Node(item);

        int cmp = item.CompareTo(node.Value);

        if (cmp < 0)
            node.Left = Insert(node.Left, item);
        else
            node.Right = Insert(node.Right, item);

        return node;
    }
    public bool Remove(T item)
    {
        int before = _count;
        _root = Delete(_root, item);
        bool removed = _count < before;
        if (removed) Dirty = true;
        return removed;
    }

    private Node? Delete(Node? node, T item)
    {
        if (node == null) return null;

        int cmp = item.CompareTo(node.Value);

        if (cmp < 0)
        {
            node.Left = Delete(node.Left, item);
        }
        else if (cmp > 0)
        {
            node.Right = Delete(node.Right, item);
        }
        else if (node.Value.Equals(item))
        {
            _count--;

            if (node.Left == null) return node.Right;  // geval 1 of 2
            if (node.Right == null) return node.Left;  // geval 2

            Node successor = FindMin(node.Right);
            node.Value = successor.Value;
            node.Right = Delete(node.Right, successor.Value);
            _count++; 
        }
        else
        {
            // Zelfde vergelijkingswaarde maar niet gelijk (duplicaat) — ga rechts
            node.Right = Delete(node.Right, item);
        }

        return node;
    }

    private Node FindMin(Node node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        return FindInOrder(_root, key, comparer);
    }

    
    private T? FindInOrder<K>(Node? node, K key, Func<T, K, bool> comparer)
    {
        if (node == null) return default;

        T? leftResult = FindInOrder(node.Left, key, comparer);
        if (leftResult != null) return leftResult;

        if (comparer(node.Value, key)) return node.Value;

        return FindInOrder(node.Right, key, comparer);
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        BinarySearchTree<T> result = new BinarySearchTree<T>();
        FilterInOrder(_root, predicate, result);
        return result;
    }

    private void FilterInOrder(Node? node, Func<T, bool> predicate, BinarySearchTree<T> result)
    {
        if (node == null) return;
        FilterInOrder(node.Left, predicate, result);
        if (predicate(node.Value)) result.Add(node.Value);
        FilterInOrder(node.Right, predicate, result);
    }

    public void Sort(Comparison<T> comparison)
    {
        T[] arr = ToArray();

        for (int i = 1; i < arr.Length; i++)
        {
            T key = arr[i];
            int j = i - 1;
            while (j >= 0 && comparison(arr[j], key) > 0)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }

        _root = null;
        _count = 0;
        BuildBalanced(arr, 0, arr.Length - 1);
        Dirty = true;
    }

    private void BuildBalanced(T[] arr, int left, int right)
    {
        if (left > right) return;
        int mid = (left + right) / 2;
        Add(arr[mid]);
        BuildBalanced(arr, left, mid - 1);
        BuildBalanced(arr, mid + 1, right);
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R result = initial;
        ReduceInOrder(_root, ref result, accumulator);
        return result;
    }

    private void ReduceInOrder<R>(Node? node, ref R result, Func<R, T, R> accumulator)
    {
        if (node == null) return;
        ReduceInOrder(node.Left, ref result, accumulator);
        result = accumulator(result, node.Value);
        ReduceInOrder(node.Right, ref result, accumulator);
    }

    public T[] ToArray()
    {
        T[] result = new T[_count];
        int index = 0;
        InOrder(_root, result, ref index);
        return result;
    }

    private void InOrder(Node? node, T[] result, ref int index)
    {
        if (node == null) return;
        InOrder(node.Left, result, ref index);
        result[index++] = node.Value;
        InOrder(node.Right, result, ref index);
    }

    public IMyIterator<T> GetIterator()
    {
        return new BSTIterator(this);
    }

    private class BSTIterator : IMyIterator<T>
    {
        private readonly T[] _sorted;
        private int _position;

        public BSTIterator(BinarySearchTree<T> tree)
        {
            _sorted = tree.ToArray();
            _position = 0;
        }

        public bool HasNext() => _position < _sorted.Length;
        public T Next() => _sorted[_position++];
        public void Reset() => _position = 0;
    }
}