using System;
using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Node: IComparable<Node>
{
    // public Coordinates Coordinates { get; private set; }
    // public Node CameFromNode{ get; private set; }
    // public bool IsWalkable{ get; private set; } = true;
    // public int ManhattanDistance{ get; private set; }

    [SerializeField]Coordinates coordinates;
    [SerializeField] bool isWalkable = true;
    [SerializeField] int estimateCost; // mangattan Distance
    int combinedCost;
    int fromStartCost;
    public TextMeshPro debugText;
    public Color currentColor;
    public Color tempColor;

    public event Action<Node> OnWalkableChanged;
    
    public Coordinates Coordinates => coordinates;
    //public Node CameFromNode => cameFromNode;
    public bool IsWalkable => isWalkable;

    public Node(Coordinates coordinates)
    {
        this.coordinates = coordinates;
        estimateCost = 0;
    }
    public void SetWalkable(bool value)
    {
        isWalkable = value;
        OnWalkableChanged?.Invoke(this);
    }
    
    public void AssignMangattanCost(int value)
    {
        estimateCost = value;
    }


    public void AssignCosts(int newFromStartCost, int newEstimateCost)
    {
        fromStartCost = newFromStartCost;
        estimateCost = newEstimateCost;
        combinedCost = fromStartCost + estimateCost;
        ChangeText();
    }
    public void CalculateCombinedCost()
    {
        combinedCost = estimateCost + fromStartCost;

        ChangeText();
    }

    public void ChangeText(string newText)
    {
        if (debugText != null)
        {
            debugText.text = newText;
        }
    }
    public void ChangeText()
    {

        ChangeText(combinedCost.ToString());
    }

    public void CreateText(TextMeshPro textCube,string text, Color textColor)
    {
        textCube.text = text;
        textCube.color = textColor;
        debugText = textCube;

        
        
    }
    public void CreateText(string text, Color textColor, float textFontSize,Vector3 position,Transform parent)
    {

        if (debugText != null)
        {
            return;
        }
        

        GameObject newText = new GameObject($"{coordinates.x};{coordinates.y}", typeof(TextMeshPro));
        newText.transform.position = position;
        TextMeshPro textMeshPro = newText.GetComponent<TextMeshPro>();
        textMeshPro.rectTransform.sizeDelta = new Vector2(10, 10);

        
        textMeshPro.text = text;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.color = textColor;
        textMeshPro.fontSize = textFontSize;
        if (parent != null)
        {
        
            textMeshPro.transform.SetParent(parent);
        }
        
        debugText = textMeshPro;

    }
    // public void ResetNode()
    // {
    //     fromStartCost = int.MaxValue;
    //     CalculateCombinedCost();
    //     debugText = null;
    //     
    // }


    public int CompareTo(Node otherNode)
    {
        if (combinedCost > otherNode.combinedCost)
        {
            return 1;
        }
        if (combinedCost < otherNode.combinedCost)
        {
            return -1;
        }
        return 0;
    }
}
