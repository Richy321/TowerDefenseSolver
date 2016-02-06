using UnityEngine;
using System.Collections;
using System;

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public int Count { get { return currentItemCount; } }

    public void UpdateItem(T item)
    {
        SortUp(item);
        //SortDown(item); - not needed as will only be increasing the value in this case(minor optimisation)
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                    Swap(item, items[swapIndex]);
                else
                    return;
            }
            else
                return;
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1)/2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
                Swap(item, parentItem);
            else
                break;
        }
    }

    void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        int aIndexTmp = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = aIndexTmp;
    }
}
