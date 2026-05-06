using Xunit;
using Project1.Collections;
using Project1.Model;

namespace Project1.Tests.Collections;

public class CollectionFactoryTests
{
    // Dit zijn de sterkste tests voor de viva:
    // Assert.IsType bewijst onomstotelijk dat de factory het juiste type aanmaakt.

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

    [Fact]
    public void GetName_RetourneertCorrecteNamen()
    {
        Assert.Equal("Generic Array",       CollectionFactory.GetName(CollectionFactory.CollectionType.Array));
        Assert.Equal("Doubly Linked List",  CollectionFactory.GetName(CollectionFactory.CollectionType.LinkedList));
        Assert.Equal("Binary Search Tree",  CollectionFactory.GetName(CollectionFactory.CollectionType.BinarySearchTree));
        Assert.Equal("Hash Map",            CollectionFactory.GetName(CollectionFactory.CollectionType.HashMap));
    }
}