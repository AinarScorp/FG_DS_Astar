using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class Player : MonoBehaviour
{
    [SerializeField] Color playerColor = Color.green;
    [SerializeField] float playerRadius = 2;
    [SerializeField]Node currentNode;
    [SerializeField]GridGenerator generator;
    CustomGrid<Node> currentGrid => generator.NodeGrid;

    [SerializeField] Pathfinding pathfinding;

    [SerializeField] float colorTIme = 2;
    List<Node> listOfNodes;

    Coroutine findingPath;
    void Awake()
    {
        DestroyItselfIfPlayerExists();
    }


    void DestroyItselfIfPlayerExists()
    {
        Player[] players = FindObjectsOfType<Player>();

        if (players.Length > 1)
        {
            Debug.Log("ss");
            DestroyImmediate(this.gameObject);
        }
    }

    // public void ReassignGrid(CustomGrid<Node> grid)
    // {
    //     currentGrid = grid;
    // }

    public void CreatePlayer(GridGenerator generator, Node startingNode)
    {
        if (!startingNode.IsWalkable)
        {
            return;
        }

        currentNode = startingNode;
        this.generator = generator;
    }


    public IEnumerator FindPath(Node endNode)
    {
        if (pathfinding ==null)
        {
            pathfinding = FindObjectOfType<Pathfinding>();
        }


        IEnumerator target = pathfinding.FindPath(currentNode.Coordinates, endNode.Coordinates);

        yield return target;
        target = pathfinding.FindPath(currentNode.Coordinates, endNode.Coordinates);
        listOfNodes = new List<Node>();
        while (target.MoveNext())
        {
            if (target.Current?.GetType() == typeof(List<Node>))
            {

                Debug.LogWarning("hereeee");
                listOfNodes = (List<Node>)target.Current;
                break;
            }
        }

        foreach (var node in listOfNodes)
        {
            generator.SetSpriteColor(node,Color.blue);
            yield return new WaitForSeconds(colorTIme);

        }
        // foreach (var node in listOfNodes)
        // {
        //     Debug.Log($"{node.Coordinates.x} : {node.Coordinates.y}");
        //     
        // }

        foreach (var node in listOfNodes)
        {
            //generator.SetSpriteColor(node,Color.cyan);
            currentNode = node;
            this.transform.position = currentGrid.GetWorldPosFromCoords(currentNode.Coordinates);
            yield return new WaitForSeconds(2);

            
        }

        findingPath = null;
    }
    public void FindPathTest(Node endNode)
    {
        if (pathfinding ==null)
        {
            pathfinding = FindObjectOfType<Pathfinding>();
        }
        
        listOfNodes = pathfinding.FindPathTest(currentNode.Coordinates, endNode.Coordinates);


        foreach (var node in listOfNodes)
        {
            generator.SetSpriteColor(node,Color.blue);
        }



    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentGrid.GetGridCoordsFromWorld(mousePos, out int x, out int y);
            
            Node destinationNode = currentGrid.GetGridType(x, y);
            if (destinationNode != null)
            {
                FindPathTest(destinationNode);
            }
            // if (destinationNode !=null)
            // {
            //     if (findingPath!=null)
            //     {
            //         StopCoroutine(findingPath);
            //         findingPath = null;
            //     }
            //     findingPath = StartCoroutine(FindPath(destinationNode));
            // }
        }
    }

    public void SetNode(Node newNode)
    {
        if (!newNode.IsWalkable)
        {
            return;
        }

        currentNode = newNode;
    }

    void OnDrawGizmos()
    {
        if (currentNode == null) return;


        Vector3 playerPos = currentGrid.GetWorldPosFromCoords(currentNode.Coordinates);

        transform.position = playerPos;
        Gizmos.color = playerColor;
        Gizmos.DrawSphere(playerPos, playerRadius);
    }
}