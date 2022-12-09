using System.Collections.Generic;
using Algorithms;
using UnityEngine;


public class Pathfinding : MonoBehaviour
{
    const int DIAGONAL_MOVE_COST = 14;
    const int STRAIGHT_MOVE_COST = 10;
    
    protected PriorityQueue<Node> openList;
    protected Dictionary<Node, int> costFromStart;
    protected Dictionary<Node, Node> cameFromNode;
    
    protected GridGenerator generator;
    protected CustomGrid<Node> nodeGrid => generator.NodeGrid;
    
    void Awake()
    {
        generator = GameObject.FindWithTag("Grid")?.GetComponent<GridGenerator>();
    }
    
    public List<Node> FindPath(Node startNode, Node endNode)
    {
        if (EncounteredProblem(startNode, endNode)) return null;

        StartNewSearch(startNode, endNode);

        while (!openList.IsEmpty())
        {
            Node currentNode = GetNewCurrentNode();

            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            foreach (Node neighbourNode in generator.GetNeighbourList(currentNode))
            {
                if (!NodeIsWalkable(neighbourNode)) continue;
                
                int newFromStartCost = GetFromStartCost(currentNode, neighbourNode);
                
                if (!costFromStart.ContainsKey(neighbourNode) || newFromStartCost < costFromStart[neighbourNode])
                {
                    AssignNewNode(currentNode,neighbourNode, endNode, newFromStartCost);
                }
            }
        }
        Debug.Log("No Path found");
        return null;
    }

    protected Node GetNewCurrentNode()
    {
        return openList.ExtractMin();
    }

    protected bool NodeIsWalkable(Node neighbourNode)
    {
        if (neighbourNode.IsWalkable) { return true;}
        
        return false;
    }

    protected void AssignNewNode(Node currentNode, Node neighbourNode, Node endNode, int newFromStartCost)
    {
        neighbourNode.AssignCosts(newFromStartCost, CalculateManhattanCost(neighbourNode, endNode));

        costFromStart[neighbourNode] = newFromStartCost;
        cameFromNode[neighbourNode] = currentNode;
        openList.Enqueue(neighbourNode);
    }

    protected int GetFromStartCost(Node currentNode, Node neighbourNode)
    {
        return costFromStart[currentNode] + CalculateManhattanCost(currentNode, neighbourNode) + neighbourNode.WalkingCost;
    }

    protected bool EncounteredProblem(Node startNode, Node endNode)
    {
        if (generator == null)
        {
            Debug.Log("GeneratorNotFound");
            return true;
        }
        if (startNode == null || endNode == null)
        {
            Debug.Log("INVALID PATH");
            return true;
        }
        return false;
    }

    protected void StartNewSearch(Node startNode, Node endNode)
    {
        startNode.AssignCosts(0, CalculateManhattanCost(startNode, endNode));
        costFromStart = new Dictionary<Node, int>() { [startNode] = 0 };
        cameFromNode = new Dictionary<Node, Node>() { [startNode] = null };
        
        openList = new PriorityQueue<Node>(nodeGrid.GridArray.Length);
        openList.Enqueue(startNode);
    }

    List<Node> CalculatePath(Node endNode)
    {
        if (cameFromNode == null)
        {
            Debug.LogError("Dictionary of the path is null");
            return null;
        }
        
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while (cameFromNode[currentNode] != null)
        {
            path.Add(cameFromNode[currentNode]);
            currentNode = cameFromNode[currentNode];
        }
        path.Reverse();

        return path;
    }

    protected int CalculateManhattanCost(Node startNode, Node endNode)
    {
        int xDistance = Mathf.Abs(startNode.Coordinates.x - endNode.Coordinates.x);
        int yDistance = Mathf.Abs(startNode.Coordinates.y - endNode.Coordinates.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        
        return DIAGONAL_MOVE_COST * Mathf.Min(xDistance,yDistance) + remaining * STRAIGHT_MOVE_COST;
    }

    

}
