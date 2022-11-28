using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithms
{
    [Serializable]
    public class MinBinaryHeap<TElement> where TElement: IComparable<TElement>
    {
        public TElement[] HeapArray;
        public int Capacity => HeapArray.Length;
        
        public int currentHeapSize = 0;
        
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
            HeapArray = new TElement[capacity];
        }

        public void Insert(TElement key)
        {
            if (currentHeapSize == Capacity)
            {
                Debug.LogError("Array is beyond capacity");
                return;
            }

            int index = currentHeapSize;

            HeapArray[index] = key;
            currentHeapSize++;

            // while (index != 0 && HeapArray[index] < HeapArray[GetParentIndex(index)])
            while (index != 0 && HeapArray[index].CompareTo(HeapArray[GetParentIndex(index)]) < 0)
            {
                Swap(ref HeapArray[index], ref HeapArray[GetParentIndex(index)]);
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
                return HeapArray[0];
            }
            
            TElement root = HeapArray[0];
  
            HeapArray[0] = HeapArray[currentHeapSize - 1];
            currentHeapSize--;
            MinHeapify(0);
  
            return root;
        }

        public TElement Peek()
        {
            if (currentHeapSize <= 0)
            {
                Debug.Log("boo");
                return default;
            }
            return HeapArray[0];
        }
        
        public void MinHeapify(int key)
        {
            int leftChildIndex = GetLeftChildIndex(key);
            int rightChildIndex = GetRightChildIndex(key);
  
            int smallest = key;
            // if (leftChildIndex < currentHeapSize && HeapArray[leftChildIndex] < HeapArray[smallest])
            if (leftChildIndex < currentHeapSize && HeapArray[leftChildIndex].CompareTo(HeapArray[smallest]) < 0)
            {
                smallest = leftChildIndex;
            }
            // if (rightChildIndex < currentHeapSize && HeapArray[rightChildIndex] < HeapArray[smallest])
            if (rightChildIndex < currentHeapSize && HeapArray[rightChildIndex].CompareTo(HeapArray[smallest]) < 0)
            {
                smallest = rightChildIndex;
            }
      
            if (smallest != key)
            {
                Swap(ref HeapArray[key], 
                    ref HeapArray[smallest]);
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

    // [Serializable]
    // public class BinaryNode
    // {
    //     public int Value;
    //     public BinaryNode LeftNode, RightNode;
    //
    //     BinaryNode(int value)
    //     {
    //         Value = value;
    //         LeftNode = RightNode = null;
    //     }
    // }
    
}


