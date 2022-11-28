using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEngine;

using TMPro;

public class Pathfinding : MonoBehaviour
{
    const int MOVE_DIAGONAL_COST = 14;
    const int MOVE_STRAIGHT_COST = 10;
    [SerializeField] float textFontSize = 10;
    [SerializeField] float timeSeconds = 0.5f;
    [SerializeField] float initialPause = 5f;
    [SerializeField] float generalWait = 2;
    CustomGrid<Node> nodeGrid => generator.NodeGrid;
    GridGenerator generator;
    PriorityQueue<Node> openList;
    Dictionary<Node, int> closedList;
    // PriorityQueue<Node> closedList;
    // List<Node> openList;
    // List<Node> closedList;

    void FindGrid()
    {
        generator = FindObjectOfType<GridGenerator>();
    }


    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
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
        generator.SetSpriteColor(startNode, Color.green);
        Node endNode = nodeGrid.GetGridType(endCoords);
        generator.SetSpriteColor(endNode, Color.blue);


        yield return Wait(initialPause);
        if (startNode == null || endNode == null)
        {
            Debug.Log("INVALID PATH");
            yield break;
        }

        
        foreach (var node in nodeGrid.GridArray)
        {
            node.ResetNode();
            
            TextMeshPro textMeshPro = Resources.Load<TextMeshPro>("TextCube");

            textMeshPro = Instantiate(textMeshPro, nodeGrid.GetWorldPosFromCoords(node.Coordinates),Quaternion.identity,this.transform);
            node.CreateText(textMeshPro,node.combinedDistance.ToString(), Color.black);
            // node.CreateText(node.combinedDistance.ToString(), Color.black,8,nodeGrid.GetWorldPosFromCoords(node.Coordinates),this.transform);
        }
        openList = new PriorityQueue<Node>(nodeGrid.GridArray.Length);
        openList.Insert(startNode);
        // closedList = new PriorityQueue<Node>(nodeGrid.GridArray.Length);
        closedList = new Dictionary<Node, int>();


        yield return Wait(generalWait);


        startNode.AssignMangattanDistance(CalculateManhattanCost(startNode, endNode));
        startNode.fromStartDistance = 0;
        startNode.CalculateCombinedDistance();
        closedList.Add(startNode,0);
        while (!openList.IsEmpty())
        {

            // Node currentNode = GetLowestCombinedCostNode(openList);
            Node currentNode = openList.ExtractMin();
            generator.SetSpriteColor(currentNode, Color.red);
            yield return Wait(generalWait);

            
            if (currentNode == endNode)
            {
                //Found path
                Debug.LogWarning("FOUND PATH");
                yield return CalculatePath(endNode);
                yield break;
            }
  

            // closedList.Insert(currentNode);
            foreach (Node neighbourNode in GetNeighbourList(currentNode))
            {
                // int newCost = closedList[currentNode] + CalculateManhattanCost(currentNode, neighbourNode);
                // if (closedList.Contains(neighbourNode))
                // {
                //     continue;
                // }
                if (!neighbourNode.IsWalkable)
                {
                    if (generator.GetSpriteColor(neighbourNode) == Color.gray)
                    {
                        print("COLORING OVER");
                    }
                    generator.SetSpriteColor(neighbourNode, Color.gray);

                    continue;
                }
                

                generator.SetSpriteColor(neighbourNode, Color.yellow);

                int newCost = closedList[currentNode] + CalculateManhattanCost(currentNode, neighbourNode);

                print( "New Cost : " + newCost);
                // int priorityCost = currentNode.fromStartDistance + CalculateManhattanCost(currentNode, neighbourNode);

                bool closedListContainsNode = closedList.ContainsKey(neighbourNode);
                print($"closed list contains NOT neighbour Node : {!closedListContainsNode}");
                print($"newCost< neighbourNode.fromStartDistance : {newCost< neighbourNode.fromStartDistance}");
                yield return Wait(generalWait);
                if (!closedListContainsNode || newCost< neighbourNode.fromStartDistance)
                {
                    print( "Got through");
                    if (closedListContainsNode)
                    {
                        closedList[neighbourNode] = newCost;
                    }
                    else
                    {
                        closedList.Add(neighbourNode,newCost);
                    }

                    neighbourNode.fromStartDistance = newCost;
                    neighbourNode.AssignCameFromNode(currentNode);
                    neighbourNode.AssignMangattanDistance(CalculateManhattanCost(neighbourNode,endNode));
                    neighbourNode.CalculateCombinedDistance();

                    openList.Insert(neighbourNode);
                    
                }
                yield return Wait(generalWait);

                // if (priorityCost<neighbourNode.fromStartDistance)
                // {
                //     neighbourNode.AssignCameFromNode(currentNode);
                //     neighbourNode.fromStartDistance = priorityCost;
                //     neighbourNode.AssignMangattanDistance(CalculateManhattanCost(neighbourNode,endNode));
                //     neighbourNode.CalculateCombinedDistance();
                //     if (!openList.Contains(neighbourNode))
                //     {
                //         openList.Add(neighbourNode);
                //     }
                // }
                
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
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance,yDistance) + remaining * MOVE_STRAIGHT_COST;
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

    // void CreateText(Node node, int cost)
    // {
    //     Vector3 pos = nodeGrid.GetWorldPosFromCoords(node.Coordinates);
    //     Vector3 posleft = nodeGrid.GetWorldPosFromCoords(node.Coordinates);
    //     posleft.x -= nodeGrid.CellSize.x/2;
    //     
    //     Vector3 posRight = nodeGrid.GetWorldPosFromCoords(node.Coordinates);
    //     posRight.x += nodeGrid.CellSize.x /2;
    //
    //     
    //     node.CreateText(node.fromStartDistance.ToString(),Color.cyan, textFontSize,posleft, this.transform);
    //     node.CreateText(node.combinedDistance.ToString(),Color.cyan, textFontSize,pos, this.transform);
    //     node.CreateText(node.ManhattanDistance.ToString(),Color.cyan, textFontSize,posRight, this.transform);
    // }

}
