using System.Collections;
using System.Collections.Generic;
using Algorithms;
using UnityEngine;

using TMPro;

public class Pathfinding : MonoBehaviour
{
    const int MOVE_DIAGONAL_COST = 14;
    const int MOVE_STRAIGHT_COST = 10;
    const string DEFAULT_TEXT = "---";
    
    [SerializeField] Color currentTileColor = Color.red;
    [SerializeField] Color wallTileColor = Color.grey;
    [SerializeField] Color neighbourTileColor = Color.yellow;
    [SerializeField] Color checkColor = Color.white;
    
    
    [SerializeField] float generalWaitTime = 2;
    
    
    
    PriorityQueue<Node> openList;
    Dictionary<Node, int> costFromStart;
    Dictionary<Node, Node> cameFromNode;

    GridGenerator generator;
    
    CustomGrid<Node> nodeGrid => generator.NodeGrid;



    void FindGrid()
    {
        generator = FindObjectOfType<GridGenerator>();
    }


    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }

    

    public IEnumerator FindPath(Node startNode, Node endNode)
    {
        if (generator == null)
        {
            FindGrid();
        }
        if (generator == null)
        {
            yield break;
        }

        if (startNode == null || endNode == null)
        {
            Debug.Log("INVALID PATH");
            yield break;
        }

        CreateNodeTexts(); // you would remove this for the actual game
        
        //Setup queue and Dict's
        openList = new PriorityQueue<Node>(nodeGrid.GridArray.Length);
        costFromStart = new Dictionary<Node, int>();
        cameFromNode = new Dictionary<Node, Node>();

        startNode.AssignCosts(0,CalculateManhattanCost(startNode, endNode));
        
        //Add first Node to the chain
        openList.Enqueue(startNode);
        cameFromNode.Add(startNode,null);
        costFromStart.Add(startNode,0);
        
        
        while (!openList.IsEmpty())
        {
            Node currentNode = openList.ExtractMin();

            generator.SetTempSpriteColor(currentNode, currentTileColor);
            yield return Wait(generalWaitTime);

            
            if (currentNode == endNode) // look into .Equals()
            {
                //Found path
                yield return CalculatePath(endNode);
                yield break;
            }
  

            foreach (Node neighbourNode in GetNeighbourList(currentNode))
            {
 
                if (!neighbourNode.IsWalkable)
                {
                    if (generator.GetSpriteColor(neighbourNode) == wallTileColor) // this is for testings, if I can manage avoid checking same tiles
                    {
                        generator.SetTempSpriteColor(neighbourNode,checkColor);
                        yield return Wait(generalWaitTime);
                    }
                    neighbourNode.ChangeText("X");
                    generator.SetSpriteColor(neighbourNode, wallTileColor);

                    continue;
                }
                

                generator.SetSpriteColor(neighbourNode,neighbourTileColor);

                int newFromStartCost = costFromStart[currentNode] + CalculateManhattanCost(currentNode, neighbourNode) + neighbourNode.WalkingCost;
                
                bool closedListContainsNode = costFromStart.ContainsKey(neighbourNode);
 
                yield return Wait(generalWaitTime);
                
                if (!closedListContainsNode || newFromStartCost< costFromStart[neighbourNode])
                {
                    costFromStart[neighbourNode] = newFromStartCost;
                    neighbourNode.AssignCosts(newFromStartCost,CalculateManhattanCost(neighbourNode,endNode));
                    cameFromNode[neighbourNode] = currentNode;
                    openList.Enqueue(neighbourNode);
                    
                }

            }
            generator.RestoreColor(currentNode);
        }
        Debug.Log("No Path found");
    }

    void CreateNodeTexts()
    {
        TextMeshPro textMeshPro = Resources.Load<TextMeshPro>("TextCube");

        generator.ResetSpriteColors();
        foreach (var node in nodeGrid.GridArray)
        {
            if (node.DebugText == null)
            {
                TextMeshPro textMeshProClone = Instantiate(textMeshPro, nodeGrid.GetWorldPosFromCoords(node.Coordinates), Quaternion.identity, this.transform);
                node.AssignDebugText(textMeshProClone, DEFAULT_TEXT, Color.black);
            }
            else
            {
                node.ChangeText(DEFAULT_TEXT);
            }
        }
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

    

}
