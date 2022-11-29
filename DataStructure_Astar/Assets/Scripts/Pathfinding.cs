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
    [SerializeField]PriorityQueue<Node> openList;
    Dictionary<Node, int> costFromStart;

    Dictionary<Node, Node> cameFromNode;




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
        yield return FindPath(startNode.Coordinates, endNode.Coordinates);
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

        
        //Why can't I pass Nodes themeselves????
        
        
        
        Node startNode = nodeGrid.GetGridType(startCoords);
        generator.SetSpriteColor(startNode, Color.green);
        Node endNode = nodeGrid.GetGridType(endCoords);
        generator.SetSpriteColor(endNode, Color.blue);


        if (startNode == null || endNode == null)
        {
            Debug.Log("INVALID PATH");
            yield break;
        }

        
        foreach (var node in nodeGrid.GridArray)
        {
            if (node.debugText == null)
            {
                TextMeshPro textMeshPro = Resources.Load<TextMeshPro>("TextCube");

                textMeshPro = Instantiate(textMeshPro, nodeGrid.GetWorldPosFromCoords(node.Coordinates),Quaternion.identity,this.transform);
                node.CreateText(textMeshPro,"---", Color.black);
            }
            else
            {
                node.ChangeText("---");
            }
        }
        openList = new PriorityQueue<Node>(nodeGrid.GridArray.Length);
        openList.Enqueue(startNode);
        costFromStart = new Dictionary<Node, int>();
        cameFromNode = new Dictionary<Node, Node>();

        
        yield return Wait(generalWait);


        startNode.AssignCosts(0,CalculateManhattanCost(startNode, endNode));
        // startNode.AssignMangattanCost(CalculateManhattanCost(startNode, endNode));
        // startNode.fromStartCost = 0;
        // startNode.CalculateCombinedCost();
        cameFromNode.Add(startNode,null);
        costFromStart.Add(startNode,0);
        while (!openList.IsEmpty())
        {

            Node currentNode = openList.ExtractMin();

            generator.SetTempSpriteColor(currentNode, Color.red);
            yield return Wait(generalWait);

            
            if (currentNode == endNode) // look into .Equals()
            {
                //Found path
                Debug.LogWarning("FOUND PATH");
                yield return CalculatePath(endNode);
                yield break;
            }
  

            foreach (Node neighbourNode in GetNeighbourList(currentNode))
            {
 
                if (!neighbourNode.IsWalkable)
                {
                    if (generator.GetSpriteColor(neighbourNode) == Color.gray)
                    {
                        print("COLORING OVER");
                        generator.SetTempSpriteColor(neighbourNode,Color.white);
                        yield return Wait(generalWait);
                    }
                    neighbourNode.ChangeText("X");
                    generator.SetSpriteColor(neighbourNode, Color.gray);

                    continue;
                }
                

                generator.SetSpriteColor(neighbourNode, Color.yellow);

                int newFromStartCost = costFromStart[currentNode] + CalculateManhattanCost(currentNode, neighbourNode);

                //print( "New Cost : " + newCost);

                bool closedListContainsNode = costFromStart.ContainsKey(neighbourNode);
 
                yield return Wait(generalWait);
                
                if (!closedListContainsNode || newFromStartCost< costFromStart[neighbourNode])
                {
                    

                    costFromStart[neighbourNode] = newFromStartCost;
                    neighbourNode.AssignCosts(newFromStartCost,CalculateManhattanCost(neighbourNode,endNode));
                    // neighbourNode.fromStartCost = newFromStartCost;
                    // neighbourNode.AssignMangattanCost(CalculateManhattanCost(neighbourNode,endNode));
                    // neighbourNode.CalculateCombinedCost();
                    cameFromNode[neighbourNode] = currentNode;

                    openList.Enqueue(neighbourNode);
                    
                }

            }
            yield return Wait(generalWait);
            generator.RestoreColor(currentNode);

        }

        Debug.Log("No Path found");
        yield break;
    }    
    public List<Node> FindPathTest(Coordinates startCoords, Coordinates endCoords)
    {
        
        if (generator == null)
        {
            FindGrid();
        }
        if (generator == null)
        {
            return null;
        }

        
        //Why can't I pass Nodes themeselves????
        
        
        
        Node startNode = nodeGrid.GetGridType(startCoords);
        generator.SetSpriteColor(startNode, Color.green);
        Node endNode = nodeGrid.GetGridType(endCoords);
        generator.SetSpriteColor(endNode, Color.blue);


        if (startNode == null || endNode == null)
        {
            Debug.Log("INVALID PATH");
            return null;
        }

        
        foreach (var node in nodeGrid.GridArray)
        {
            if (node.debugText == null)
            {
                TextMeshPro textMeshPro = Resources.Load<TextMeshPro>("TextCube");

                textMeshPro = Instantiate(textMeshPro, nodeGrid.GetWorldPosFromCoords(node.Coordinates),Quaternion.identity,this.transform);
                node.CreateText(textMeshPro,"---", Color.black);
            }
            else
            {
                node.ChangeText("---");
            }
        }
        openList = new PriorityQueue<Node>(nodeGrid.GridArray.Length);
        openList.Enqueue(startNode);
        costFromStart = new Dictionary<Node, int>();
        cameFromNode = new Dictionary<Node, Node>();

        


        startNode.AssignCosts(0,CalculateManhattanCost(startNode, endNode));
        // startNode.AssignMangattanCost(CalculateManhattanCost(startNode, endNode));
        // startNode.fromStartCost = 0;
        // startNode.CalculateCombinedCost();
        cameFromNode.Add(startNode,null);
        costFromStart.Add(startNode,0);
        while (!openList.IsEmpty())
        {

            Node currentNode = openList.ExtractMin();

            generator.SetTempSpriteColor(currentNode, Color.red);

            
            if (currentNode == endNode) // look into .Equals()
            {
                //Found path
                Debug.LogWarning("FOUND PATH");
                return CalculatePath(endNode);

            }
  

            foreach (Node neighbourNode in GetNeighbourList(currentNode))
            {
 
                if (!neighbourNode.IsWalkable)
                {
                    if (generator.GetSpriteColor(neighbourNode) == Color.gray)
                    {
                        print("COLORING OVER");
                        generator.SetTempSpriteColor(neighbourNode,Color.white);
                    }
                    neighbourNode.ChangeText("X");
                    generator.SetSpriteColor(neighbourNode, Color.gray);

                    continue;
                }
                

                generator.SetSpriteColor(neighbourNode, Color.yellow);

                int newFromStartCost = costFromStart[currentNode] + CalculateManhattanCost(currentNode, neighbourNode);

                //print( "New Cost : " + newCost);

                bool closedListContainsNode = costFromStart.ContainsKey(neighbourNode);
 
                
                if (!closedListContainsNode || newFromStartCost< costFromStart[neighbourNode])
                {
                    

                    costFromStart[neighbourNode] = newFromStartCost;
                    neighbourNode.AssignCosts(newFromStartCost,CalculateManhattanCost(neighbourNode,endNode));
                    // neighbourNode.fromStartCost = newFromStartCost;
                    // neighbourNode.AssignMangattanCost(CalculateManhattanCost(neighbourNode,endNode));
                    // neighbourNode.CalculateCombinedCost();
                    cameFromNode[neighbourNode] = currentNode;

                    openList.Enqueue(neighbourNode);
                    
                }

            }
            generator.RestoreColor(currentNode);

        }

        Debug.Log("No Path found");
        return null;
    }
    List<Node> CalculatePath(Node endNode)
    {
        if (cameFromNode == null)
        {
            Debug.LogError("Dictionary of the path is null");
            return null;
        }
        
        List<Node> path = new List<Node>();
        path.Add(cameFromNode[endNode]);
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
    //     node.CreateText(node.fromStartCost.ToString(),Color.cyan, textFontSize,posleft, this.transform);
    //     node.CreateText(node.combinedCost.ToString(),Color.cyan, textFontSize,pos, this.transform);
    //     node.CreateText(node.ManhattanDistance.ToString(),Color.cyan, textFontSize,posRight, this.transform);
    // }

}
