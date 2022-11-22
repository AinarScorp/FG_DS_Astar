using System;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Node
{
    // public Coordinates Coordinates { get; private set; }
    // public Node CameFromNode{ get; private set; }
    // public bool IsWalkable{ get; private set; } = true;
    // public int ManhattanDistance{ get; private set; }

    [SerializeField]Coordinates coordinates;
    [SerializeField]Node cameFromNode;
    [SerializeField] bool isWalkable = true;
    [SerializeField] int manhattanDistance;
    public int combinedDistance;
    public int fromStartDistance;
    public TextMeshPro debugText;

    public event Action<Node> OnWalkableChanged;
    
    public Coordinates Coordinates => coordinates;
    public Node CameFromNode => cameFromNode;
    public bool IsWalkable => isWalkable;
    public int ManhattanDistance => manhattanDistance;

    public Node(Coordinates coordinates)
    {
        this.coordinates = coordinates;
        manhattanDistance = 0;
    }
    public void SetWalkable(bool value)
    {
        isWalkable = value;
        OnWalkableChanged?.Invoke(this);
    }
    
    public void AssignMangattanDistance(int value)
    {
        manhattanDistance = value;
    }

    public void AssignCameFromNode(Node newNode)
    {
        cameFromNode = newNode;
    }

    public void CalculateCombinedDistance()
    {
        combinedDistance = manhattanDistance + fromStartDistance;
    }

    public void CreateText(string text, Color textColor, float textFontSize,Vector3 position,Transform parent)
    {
                
        
        GameObject newText = new GameObject($"{coordinates.x};{coordinates.y}", typeof(TextMeshPro));
        newText.transform.position = position;
        TextMeshPro textMeshPro = newText.GetComponent<TextMeshPro>();
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
    public void ResetNode()
    {
        fromStartDistance = int.MaxValue;
        CalculateCombinedDistance();
        cameFromNode = null;

        debugText = null;
    }
}
