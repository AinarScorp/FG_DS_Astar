using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TwoDimensionalArraySerializer<T>:ISerializationCallbackReceiver
{
    [SerializeField] int width, height;
    [SerializeField] List<DataToSerialise<T>> storedGridArrayElements;

    T[,] multiArray;

    public List<DataToSerialise<T>> StoredGridArrayElements => storedGridArrayElements;
    public T[,] MultiArray => multiArray;

    public TwoDimensionalArraySerializer(T[,] multiArray)
    {
        this.multiArray = multiArray;
        width = multiArray.GetLength(0);
        height = multiArray.GetLength(1);
    }
    public void OnBeforeSerialize()
    {
        storedGridArrayElements = new List<DataToSerialise<T>>();
        
        for (int x = 0; x < multiArray.GetLength(0); x++)
        {
            for (int y = 0; y < multiArray.GetLength(1); y++)
            {
                storedGridArrayElements.Add(new DataToSerialise<T>(x, y, multiArray[x, y]));
            }
        }
    }
    
    public void OnAfterDeserialize()
    {
        multiArray = new T[width, height];
        
        foreach(var package in storedGridArrayElements)
        {
            multiArray[package.Index0, package.Index1] = package.Element;
        }
    }
}

[Serializable]
public struct DataToSerialise<TElement>
{
    public int Index0; //x
    public int Index1; //y
    public TElement Element;
    public DataToSerialise(int idx0, int idx1, TElement element)
    {
        Index0 = idx0;
        Index1 = idx1;
        Element = element;
    }
}
