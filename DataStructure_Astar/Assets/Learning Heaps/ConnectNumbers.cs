using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConnectNumbers : MonoBehaviour
{
    [SerializeField] GameObject[] numbers;
    [SerializeField] float lineThickness = 2;
    [SerializeField] Color lineColor = Color.white;
    [SerializeField] float lineCut = 1;

    void OnDrawGizmos()
    {
        if (numbers == null)
        {
            return;
        }
        Handles.color = lineColor;
        foreach (var number in numbers)
        {
            Vector3 from = transform.position;
            Vector3 to = number.transform.position;
            Vector3 direction = (to - from).normalized;
            from += direction * lineCut;
            to -=direction * lineCut;
            Handles.DrawLine(from, to, lineThickness);
        }
    }
}
