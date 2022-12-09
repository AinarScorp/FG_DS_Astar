using System;
using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEngine;

namespace Algorithms
{
    [Serializable]
    public class PriorityQueue<TElement> where TElement : IComparable<TElement>
    {

        MinBinaryHeap<TElement> heap;

        public PriorityQueue(int capacity)
        {
            heap = new MinBinaryHeap<TElement>(capacity);
        }

        public TElement Peek()
        {
            return heap.Peek();
        }
        public void Enqueue(TElement key)
        {
            heap.Insert(key);
        }
        public TElement ExtractMin()
        {
           return heap.ExtractMin();
        }

        public bool IsEmpty()
        {
            return heap.CurrentHeapSize < 1;
        }


    }
    
}
