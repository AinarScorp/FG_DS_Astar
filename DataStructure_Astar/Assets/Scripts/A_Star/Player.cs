using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Player : MonoBehaviour
{
    //Player info
    [SerializeField] Color playerColor = Color.green;
    [SerializeField] float playerRadius = 2;
    [SerializeField] float playerStepTime = 0.5f;
    
    //Found path Info
    [SerializeField] float colorTime = 2;
    [SerializeField] Color foundPathColor = Color.cyan;
    
    
    [SerializeField] Coordinates currentCoordinates;
    [SerializeField] GridGenerator generator;

    [SerializeField] Pathfinding pathfinding;
    CustomGrid<Node> currentGrid => generator.NodeGrid;
    
    List<Node> listOfNodes;
    Coroutine findingPath;

    #region Setup

    

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
    #endregion

    void Update()
    {
        StartPathfinding();
    }
    
    
    public void CreatePlayer(GridGenerator generator, Node startingNode)
    {
        if (!startingNode.IsWalkable)
        {
            return;
        }

        currentCoordinates = startingNode.Coordinates;
        this.generator = generator;
    }


    public IEnumerator FindPath(Node endNode)
    {
        if (pathfinding ==null)
        {
            pathfinding = FindObjectOfType<Pathfinding>();
        }


        Node currentNode = currentGrid.GetGridType(currentCoordinates);
        IEnumerator target = pathfinding.FindPath(currentNode, endNode);
        yield return target;
        target = pathfinding.FindPath(currentNode, endNode);
        listOfNodes = new List<Node>();
        while (target.MoveNext())
        {
            if (target.Current?.GetType() == typeof(List<Node>))
            {
        
                listOfNodes = (List<Node>)target.Current;
                break;
            }
        }
        
        foreach (var node in listOfNodes)
        {
            generator.SetSpriteColor(node,foundPathColor);
            yield return new WaitForSeconds(colorTime);
        }
        foreach (var node in listOfNodes)
        {
            currentCoordinates = node.Coordinates;
            this.transform.position = currentGrid.GetWorldPosFromCoords(currentCoordinates);
            yield return new WaitForSeconds(playerStepTime);
        }

        findingPath = null;
    }





    void StartPathfinding()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentGrid.GetGridCoordsFromWorld(mousePos, out int x, out int y);

            Node destinationNode = currentGrid.GetGridType(x, y);

            if (destinationNode != null)
            {
                if (findingPath != null)
                {
                    StopCoroutine(findingPath);
                    findingPath = null;
                }

                findingPath = StartCoroutine(FindPath(destinationNode));
            }
        }
    }


    void OnDrawGizmos()
    {
        Node currentNode = currentGrid.GetGridType(currentCoordinates);

        if (currentNode == null) return;
        
        Vector3 playerPos = currentGrid.GetWorldPosFromCoords(currentNode.Coordinates);

        Gizmos.color = playerColor;
        Gizmos.DrawSphere(playerPos, playerRadius);
    }
}