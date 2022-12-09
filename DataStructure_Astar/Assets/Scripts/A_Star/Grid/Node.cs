using System;
using UnityEngine;

[Serializable]
public class Node: IComparable<Node>
{
    [SerializeField] int combinedCost;
    [SerializeField] int heuristicCost;
    [SerializeField] Coordinates coordinates;
    
    
    [SerializeField] TileType tileType;
    [SerializeField] TileType previousTileType; //Undo imitation
    
    [SerializeField] NodeVisualiser visualiser;


    public event Action<Node> OnTileTypeChanged;

    #region Properties
    public int WalkingCost => tileType.WalkingCost;
    public bool IsWalkable => tileType.IsWalkable;
    public Color TileColor => tileType.TypeColor;
    public Coordinates Coordinates => coordinates;
    public NodeVisualiser Visualiser => visualiser;

    #endregion


    public Node(Coordinates coordinates, TileType tileType)
    {
        this.tileType = tileType;
        previousTileType = tileType;
        this.coordinates = coordinates;
        visualiser = new NodeVisualiser();
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
        heuristicCost = newEstimateCost;
        int fromStartCost = newFromStartCost;
        int estimatedCost = newEstimateCost;
        combinedCost = fromStartCost + estimatedCost;
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
        if (heuristicCost > otherNode.heuristicCost)
        {
            return 1;
        }
        if (heuristicCost < otherNode.heuristicCost)
        {
            return -1;
        }
        return 0;
    }
}
