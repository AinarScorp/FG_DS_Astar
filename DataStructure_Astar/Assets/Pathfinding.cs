using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] float textFontSize = 10;
    [SerializeField] float timeSeconds = 0.5f;
    CustomGrid<Node> nodeGrid => generator.NodeGrid;
    GridGenerator generator;
    List<Node> openList;
    List<Node> closedList;

    void FindGrid()
    {
        generator = FindObjectOfType<GridGenerator>();
    }


    public IEnumerator FindPath(Coordinates startCoords, Coordinates endCoords)
    {
        
        if (generator == null)
        {
            FindGrid();
        }
        if (generator == null)
        {
            yield break;
        }

        Node startNode = nodeGrid.GetGridType(startCoords);
        Node endNode = nodeGrid.GetGridType(endCoords);
        if (startNode == null || endNode == null)
        {
            Debug.Log("INVALID PATH");
            yield break;
        }

        
        foreach (var node in nodeGrid.GridArray)
        {
            node.ResetNode();
        }
        openList = new List<Node> { startNode };
        closedList = new List<Node>();

        startNode.AssignMangattanDistance(CalculateManhattanCost(startNode, endNode));
        startNode.fromStartDistance = 0;
        startNode.CalculateCombinedDistance();
        while (openList.Count>0)
        {

            Node currentNode = GetLowestCombinedCostNode(openList);

            if (currentNode == endNode)
            {
                //Found path
                yield return CalculatePath(endNode);
                yield break;
            }
  

            openList.Remove(currentNode);
            closedList.Add(currentNode);
            foreach (Node neighbourNode in GetNeighbourList(currentNode))
            {
                yield return new WaitForSeconds(timeSeconds);
                if (closedList.Contains(neighbourNode))
                {
                    if (neighbourNode.IsWalkable)
                    {
                        generator.SetSpriteColor(neighbourNode, Color.green);
                        
                    }
                    continue;
                }
                if (!neighbourNode.IsWalkable)
                {
                    closedList.Add(neighbourNode);
                    generator.SetSpriteColor(neighbourNode, Color.red);
                    continue;
                }


                generator.SetSpriteColor(neighbourNode, Color.yellow);
                int priorityCost = currentNode.fromStartDistance + CalculateManhattanCost(currentNode, neighbourNode);
 
                if (priorityCost<neighbourNode.fromStartDistance)
                {
                    neighbourNode.AssignCameFromNode(currentNode);
                    neighbourNode.fromStartDistance = priorityCost;
                    neighbourNode.AssignMangattanDistance(CalculateManhattanCost(neighbourNode,endNode));
                    neighbourNode.CalculateCombinedDistance();
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                    generator.SetSpriteColor(neighbourNode,Color.blue);
                }
                
            }

        }

        Debug.Log("No Path found");
        yield break;
    }

    List<Node> CalculatePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while (currentNode.CameFromNode != null)
        {
            path.Add(currentNode.CameFromNode);
            currentNode = currentNode.CameFromNode;

        }
        path.Reverse();
        return path;

    }
    List<Node> GetNeighbourList(Node currentNode)
    {
        List<Node> neighbourList = new List<Node>();
        Coordinates currentNodeCoords = currentNode.Coordinates;

        bool thereIsUpperNode = currentNodeCoords.y + 1 < nodeGrid.Height;
        bool thereIsBottomNode = currentNodeCoords.y - 1 >= 0;
        bool thereIsRightNode = currentNodeCoords.x + 1 <nodeGrid.Width;
        bool thereIsLeftNode = currentNodeCoords.x - 1 >= 0;

        if (thereIsUpperNode)
        {

            neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x,currentNodeCoords.y+1));
            if (thereIsRightNode)
            {
                neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x+1,currentNodeCoords.y+1));
            }

            if (thereIsLeftNode)
            {
                neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x-1,currentNodeCoords.y+1));

            }
            
        }
        if (thereIsBottomNode)
        {

            neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x,currentNodeCoords.y-1));
            if (thereIsRightNode)
            {
                neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x+1,currentNodeCoords.y-1));
            }

            if (thereIsLeftNode)
            {
                neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x-1,currentNodeCoords.y-1));

            }
        }
        if (thereIsRightNode)
        {

            neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x+1,currentNodeCoords.y));
        }
        if (thereIsLeftNode)
        {
            neighbourList.Add(nodeGrid.GetGridType(currentNodeCoords.x-1,currentNodeCoords.y));
        }

        return neighbourList;
    }

    int CalculateManhattanCost(Node startNode, Node endNode)
    {
        int xDistance = Mathf.Abs(startNode.Coordinates.x - endNode.Coordinates.x);
        int yDistance = Mathf.Abs(startNode.Coordinates.y - endNode.Coordinates.y);
        return xDistance + yDistance;
    }
    Node GetLowestCombinedCostNode(List<Node> nodeList)
    {
        Node lowestCombinedCostNode = nodeList[0];
        for (int i = 1; i < nodeList.Count; i++)
        {
            if (nodeList[i].combinedDistance < lowestCombinedCostNode.combinedDistance)
            {
                lowestCombinedCostNode = nodeList[i];
            }
        }
        return lowestCombinedCostNode;
    }

    void CreateText(Node node, int cost)
    {
        Vector3 pos = nodeGrid.GetWorldPosFromCoords(node.Coordinates);
        Vector3 posleft = nodeGrid.GetWorldPosFromCoords(node.Coordinates);
        posleft.x -= nodeGrid.CellSize.x/2;
        
        Vector3 posRight = nodeGrid.GetWorldPosFromCoords(node.Coordinates);
        posRight.x += nodeGrid.CellSize.x /2;

        
        node.CreateText(node.fromStartDistance.ToString(),Color.cyan, textFontSize,posleft, this.transform);
        node.CreateText(node.combinedDistance.ToString(),Color.cyan, textFontSize,pos, this.transform);
        node.CreateText(node.ManhattanDistance.ToString(),Color.cyan, textFontSize,posRight, this.transform);
    }

}
