using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class BinaryHeapParent : MonoBehaviour
{
    [SerializeField] bool execute;
    [SerializeField] BubbleNumber prefabNumber;
    [SerializeField] [Range(0, 100)] int arrayLength = 1;
    [SerializeField] int[] numbers;


    [SerializeField] Vector2 offsetPos;


    [SerializeField] BubbleNumber[] bubbleNumbers;
    [SerializeField] float lineThickness = 2;
    [SerializeField] Color lineColor = Color.white;
    [SerializeField] float lineCut = 1;
    [SerializeField] float seconds = 2;


    [SerializeField] float speed = 2;

    void OnDrawGizmos()
    {
        ConnectBubbles();
    }

    void ConnectBubbles()
    {
        if (bubbleNumbers == null)
        {
            return;
        }

        Handles.color = lineColor;
        int nonLeafAmount = bubbleNumbers.Length / 2 - 1;
        for (int i = 0; i <= nonLeafAmount; i++)
        {
            int leftBubble = 2 * i + 1;
            int rightBubble = 2 * i + 2;
            Vector3 from = bubbleNumbers[i].transform.position;

            Vector3 toLeft = bubbleNumbers[leftBubble].transform.position;

            DrawLine(from, toLeft);

            if (rightBubble < bubbleNumbers.Length)
            {
                Vector3 toRight = bubbleNumbers[rightBubble].transform.position;
                DrawLine(from, toRight);
            }
        }
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        from += direction * lineCut;
        to -= direction * lineCut;

        Handles.DrawLine(from, to, lineThickness);
    }

    void Update()
    {
        if (execute)
        {
            execute = false;
            DeleteAllObjects();

            CreateBubbles();
        }
    }

    void CreateBubbles()
    {
        CreateRandomArray();
        int size = numbers.Length;
        bubbleNumbers = new BubbleNumber[size];
        for (int i = 0; i < size; i++)
        {
            Vector3 position = GetNodePosition(i, size);
            BubbleNumber newBubble = Instantiate(prefabNumber, position, quaternion.identity, this.transform);
            newBubble.SetName(numbers[i]);
            bubbleNumbers[i] = newBubble;
        }
    }

    Vector3 GetNodePosition(int i, int size)
    {
        Vector3 position = transform.position;
        int row = Mathf.FloorToInt(Mathf.Log(i + 1, 2));

        float yPos = offsetPos.y * row;
        position.y = yPos;
        bool numberIsEven = (i % 2 == 0);
        int parentBubbleIndex = numberIsEven ? (i - 2) / 2 : (i - 1) / 2;
        if (parentBubbleIndex < 0)
        {
            return position;
        }

        Vector3 parentPos = bubbleNumbers[parentBubbleIndex].transform.position;

        int sign = numberIsEven ? 1 : -1;
        float sizeOffset = size /(float)row;

        float xPos = parentPos.x +  offsetPos.x* sign * sizeOffset*2;
        position.x = xPos;


        //Vector3 position = transform.position + Vector3.right * (offsetPos * i);

        return position;
    }

    [ContextMenu("Delete All")]
    void DeleteAllObjects()
    {
        Transform parent = this.transform;
        List<GameObject> listToDestroy = new List<GameObject>();
        foreach (Transform child in parent)
        {
            listToDestroy.Add(child.gameObject);
        }

        foreach (var child in listToDestroy)
        {
            DestroyImmediate(child.gameObject);
        }

        bubbleNumbers = null;
    }

    [ContextMenu("Generate Random Numers")]
    void CreateRandomArray()
    {
        numbers = new int[arrayLength];
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = Random.Range(0, 100);
            if (bubbleNumbers != null)
            {
                bubbleNumbers[i].SetName(numbers[i]);
            }
        }
    }

    [ContextMenu("Heapify")]
    void Heapify()
    {
        StartCoroutine(CompleteHeapify());
    }

    IEnumerator CompleteHeapify()
    {
        int nonLeafAmount = bubbleNumbers.Length / 2 - 1;
        for (int i = nonLeafAmount; i >= 0; i--)
        {
            yield return Heapifying(i);
        }
    }

    IEnumerator Heapifying(int i)
    {
        int size = bubbleNumbers.Length;
        int largest = i;
        int leftBubble = 2 * i + 1;
        int rightBubble = 2 * i + 2;

        Debug.Log(bubbleNumbers[largest].Number);
        if (leftBubble < size && bubbleNumbers[leftBubble].Number > bubbleNumbers[largest].Number)
        {
            largest = leftBubble;
        }

        if (rightBubble < size && bubbleNumbers[rightBubble].Number > bubbleNumbers[largest].Number)
        {
            largest = rightBubble;
        }

        if (largest != i)
        {
            Debug.Log($"changing {bubbleNumbers[largest].Number}  and {bubbleNumbers[i].Number}");

            float percent = 0;
            Vector3 startPos01 = bubbleNumbers[largest].transform.position;
            Vector3 startPos02 = bubbleNumbers[i].transform.position;
            bubbleNumbers[largest].SetColor(Color.red);
            bubbleNumbers[i].SetColor(Color.red);
            yield return new WaitForSeconds(seconds);
            while (percent < 1)
            {
                percent += Time.deltaTime * speed;
                bubbleNumbers[largest].transform.position = Vector3.Lerp(startPos01, startPos02, percent);
                bubbleNumbers[i].transform.position = Vector3.Lerp(startPos02, startPos01, percent);
                yield return null;
            }

            (bubbleNumbers[largest], bubbleNumbers[i]) = (bubbleNumbers[i], bubbleNumbers[largest]);

            
            //same Thing???
            // BubbleNumber temp = bubbleNumbers[largest];
            // bubbleNumbers[largest] = bubbleNumbers[i];
            //
            // bubbleNumbers[i] = temp;
            bubbleNumbers[largest].ResetColor();
            bubbleNumbers[i].ResetColor();
            yield return Heapifying(largest);
        }
    }
}