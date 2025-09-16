using System;
using UnityEngine;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data = new List<T>();

    public int Count => data.Count;

    // Adds an item to the queue
    public void Enqueue(T item)
    {
        data.Add(item);

        int childIndex = data.Count - 1;

        while(childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;

            if (data[childIndex].CompareTo(data[parentIndex]) >= 0) break;
            (data[childIndex], data[parentIndex]) = (data[parentIndex], data[childIndex]);

            childIndex = parentIndex;
        }
    }

    // Removes an item from the queue
    public T Dequeue()
    {
        int lastIndex = data.Count - 1;
        T frontItem = data[0];
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);

        int parentIndex = 0;

        while (true)
        {
            int childIndex = parentIndex * 2 + 1;

            if(childIndex >= data.Count) break;

            int rightChild = childIndex + 1;

            if(rightChild < data.Count && data[rightChild].CompareTo(data[childIndex]) < 0) childIndex = rightChild;

            if (data[parentIndex].CompareTo(data[childIndex]) <= 0) break;
            (data[parentIndex], data[childIndex]) = (data[childIndex], data[parentIndex]);
            parentIndex = childIndex;
        }

        return frontItem;
    }
}
