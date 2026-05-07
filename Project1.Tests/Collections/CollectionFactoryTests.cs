using Xunit;
using Project1.Collections;
using Project1.Model;

namespace Project1.Tests.Collections;

public class CollectionFactoryTests
{
    [Fact]
    public void Create_Keuze1_GeeftGenericArray()
    {
        var collectie = CollectionFactory.Create<TaskItem>(CollectionFactory.CollectionType.Array);
        Assert.IsType<GenericArray<TaskItem>>(collectie);
    }

    [Fact]
    public void Create_Keuze2_GeeftDoublyLinkedList()
    {
        var collectie = CollectionFactory.Create<TaskItem>(CollectionFactory.CollectionType.LinkedList);
        Assert.IsType<DoublyLinkedList<TaskItem>>(collectie);
    }

    [Fact]
    public void Create_Keuze3_GeeftBinarySearchTree()
    {
        var collectie = CollectionFactory.Create<TaskItem>(CollectionFactory.CollectionType.BinarySearchTree);
        Assert.IsType<BinarySearchTree<TaskItem>>(collectie);
    }

    [Fact]
    public void Create_Keuze4_GeeftMyHashMap()
    {
        var collectie = CollectionFactory.Create<TaskItem>(CollectionFactory.CollectionType.HashMap);
        Assert.IsType<MyHashMap<TaskItem>>(collectie);
    }
}
