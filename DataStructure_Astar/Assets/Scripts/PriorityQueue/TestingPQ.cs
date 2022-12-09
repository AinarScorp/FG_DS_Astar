using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class TestingPQ : MonoBehaviour
{
    public int[] array;
    [SerializeField] [Range(0, 100)] int arrayLength = 1;

    PriorityQueue<int> priorityQueue;

    [ContextMenu("Execute")]
    void Execute()
    {
        priorityQueue = new PriorityQueue<int>(array.Length);
        foreach (var integet in array)
        {
            priorityQueue.Enqueue(integet);
        }

        for (int i = 0; i < array.Length; i++)
        {
            int number = priorityQueue.ExtractMin();
            print(number);
        }

    }
    [ContextMenu("Generate Random Numbers")]
    void CreateRandomArray()
    {
        array = new int[arrayLength];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Random.Range(0, 100);
        }
    }
    
}


