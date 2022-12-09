using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithms
{
    [Serializable]
    public class MinBinaryHeap<TElement> where TElement: IComparable<TElement>
    { 
        int currentHeapSize = 0;
        TElement[] heapArray;
        int capacity => heapArray.Length;
        public int CurrentHeapSize => currentHeapSize;
        
        int GetParentIndex(int index)
        {
            return (index - 1) / 2;
        }
        int GetLeftChildIndex(int index)
        {
            return 2 * index + 1;
        }

        int GetRightChildIndex(int index)
        {
            return 2 * index + 2;
        }

        public MinBinaryHeap(int capacity)
        {
            heapArray = new TElement[capacity];
        }

        public void Insert(TElement key)
        {
            if (currentHeapSize == capacity)
            {
                Debug.LogError("Array is beyond capacity");
                return;
            }

            int index = currentHeapSize;

            heapArray[index] = key;
            currentHeapSize++;

            while (index != 0 && heapArray[index].CompareTo(heapArray[GetParentIndex(index)]) < 0)
            {
                Swap(ref heapArray[index], ref heapArray[GetParentIndex(index)]);
                index = GetParentIndex(index);
            }
        }
        
        public TElement ExtractMin()
        {
            if (currentHeapSize <= 0)
            {
                return default;
            }
            if (currentHeapSize == 1)
            {
                currentHeapSize--;
                return heapArray[0];
            }
            
            TElement root = heapArray[0];
  
            heapArray[0] = heapArray[currentHeapSize - 1];
            currentHeapSize--;
            MinHeapify(0);
  
            return root;
        }

        public TElement Peek()
        {
            if (currentHeapSize <= 0)
            {
                return default;
            }
            return heapArray[0];
        }
        
        public void MinHeapify(int key)
        {
            int leftChildIndex = GetLeftChildIndex(key);
            int rightChildIndex = GetRightChildIndex(key);
  
            int smallest = key;
            if (leftChildIndex < currentHeapSize && heapArray[leftChildIndex].CompareTo(heapArray[smallest]) < 0)
            {
                smallest = leftChildIndex;
            }
            if (rightChildIndex < currentHeapSize && heapArray[rightChildIndex].CompareTo(heapArray[smallest]) < 0)
            {
                smallest = rightChildIndex;
            }
      
            if (smallest != key)
            {
                Swap(ref heapArray[key], 
                    ref heapArray[smallest]);
                MinHeapify(smallest);
            }
        }
        public  void Swap<T>(ref T leftSide, ref T rightSide)
        {
            T temp = leftSide;
            leftSide = rightSide;
            rightSide = temp;
        }
    }
}


