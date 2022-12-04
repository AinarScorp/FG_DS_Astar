using System;
using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Node: IComparable<Node>
{

    [SerializeField] int estimateCost = 0; // mangattan Distance
    [SerializeField] int fromStartCost;
    [SerializeField] int combinedCost;
    [SerializeField] Coordinates coordinates;
    
    
    [SerializeField] TileType tileType;
    [SerializeField] TileType previousTileType;
    
    [SerializeField]TextMeshPro debugText;
    
    Color currentColor;
    

    public event Action<Node> OnTileTypeChanged;

    #region Properties
    public int WalkingCost => tileType.WalkingCost;
    public bool IsWalkable => tileType.IsWalkable;
    public Color TileColor => tileType.TypeColor;
    public Color CurrentColor => currentColor;
    public Coordinates Coordinates => coordinates;
    public TextMeshPro DebugText => debugText;
    #endregion


    public Node(Coordinates coordinates, TileType tileType)
    {
        this.tileType = tileType;
        previousTileType = tileType;
        this.coordinates = coordinates;
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

        string newText = $"S:{fromStartCost} H:{estimateCost} F:{combinedCost}";
        ChangeText(newText);
    }


    #region Debug Texts
    
    public void AssignDebugText(TextMeshPro textCube,string text, Color textColor)
    {
        textCube.text = text;
        textCube.color = textColor;
        debugText = textCube;
        
    }
    public void ChangeText(string newText)
    {
        if (debugText != null)
        {
            debugText.text = newText;
        }
    }

    #endregion

    public void SetCurrentColor(Color newCurrentColor)
    {
        currentColor = newCurrentColor;
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
