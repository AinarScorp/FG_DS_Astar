using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Player : MonoBehaviour
{
    [SerializeField] Color playerColor = Color.green;
    [SerializeField] float playerRadius = 2;
    Node currentNode;
    CustomGrid<Node> currentGrid;

    [SerializeField] Pathfinding pathfinding;


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

    public void ReassignGrid(CustomGrid<Node> grid)
    {
        currentGrid = grid;
    }

    public void CreatePlayer(CustomGrid<Node> grid, Node startingNode)
    {
        if (!startingNode.IsWalkable)
        {
            return;
        }

        currentNode = startingNode;
        this.currentGrid = grid;
    }


    public IEnumerator FindPath(Node endNode)
    {
        List<Node> listOfNodes = pathfinding.FindPath(currentNode.Coordinates, endNode.Coordinates);
        // foreach (var node in listOfNodes)
        // {
        //     Debug.Log($"{node.Coordinates.x} : {node.Coordinates.y}");
        //     
        // }

        yield return new WaitForSeconds(2);
        foreach (var node in listOfNodes)
        {
            currentNode = node;
            yield return new WaitForSeconds(2);

            
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(mousePos);
            currentGrid.GetGridCoordsFromWorld(mousePos, out int x, out int y);
            currentNode = currentGrid.GetGridType(x, y);
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

        Gizmos.color = playerColor;
        Gizmos.DrawSphere(playerPos, playerRadius);
    }
}