using System;
using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Node: IComparable<Node>
{

    [SerializeField]Coordinates coordinates;
    [SerializeField] int estimateCost; // mangattan Distance
    int combinedCost;
    int fromStartCost;
    public TextMeshPro debugText;
    public Color currentColor;
    public Color tempColor;

    [SerializeField]TileType tileType;
    [SerializeField]TileType previousTileType;

    public event Action<Node> OnTileTypeChanged;

    public int WalkingCost => tileType.WalkingCost;
    public Color TileColor => tileType.TypeColor;
    public Coordinates Coordinates => coordinates;
    public bool IsWalkable => tileType.IsWalkable;

    public Node(Coordinates coordinates, TileType tileType)
    {
        this.tileType = tileType;
        previousTileType = tileType;
        this.coordinates = coordinates;
        estimateCost = 0;
    }

    public void AssignTileType(TileType newTileType)
    {
        if (tileType == newTileType)
        {
            tileType = previousTileType;
        }
        else
        {
            previousTileType = tileType;
            tileType = newTileType;
        }
        OnTileTypeChanged?.Invoke(this);
    }

    
    public void AssignCosts(int newFromStartCost, int newEstimateCost)
    {
        fromStartCost = newFromStartCost;
        estimateCost = newEstimateCost;
        combinedCost = fromStartCost + estimateCost;
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
