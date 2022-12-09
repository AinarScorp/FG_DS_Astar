using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerInfo playerInfo;

    [SerializeField] float playerStepTime = 1f;
    [SerializeField] float pathColoringTime = 1f;
    [SerializeField,HideInInspector] GridGenerator generator;
    [SerializeField,HideInInspector] Coordinates currentCoordinates;

    //cached
    PathfindingVisualiser pathfindingVisualiser;
    Camera mainCam;
    
    Coroutine findingPath;
    CustomGrid<Node> currentGrid => generator.NodeGrid;

    #region Setup
    void Awake()
    {
        pathfindingVisualiser = FindObjectOfType<PathfindingVisualiser>();
        mainCam = Camera.main;
    }
    void Start()
    {
        SetupPlayer();
    }
    void SetupPlayer()
    {
        transform.position = currentGrid.GetWorldPosFromCoords(currentCoordinates);
        transform.localScale *= playerInfo.PlayerRadius *2;
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material.color = playerInfo.PlayerColor;
    }
    public void GeneratePlayer(GridGenerator generator, Node startingNode)
    {
        if (!startingNode.IsWalkable)
        {
            return;
        }
        currentCoordinates = startingNode.Coordinates;
        this.generator = generator;
    }
    #endregion

    void Update()
    {
        StartPathfinding();
    }
    IEnumerator FindPath(Node endNode)
    {
        Node currentNode = currentGrid.GetGridType(currentCoordinates);
        yield return pathfindingVisualiser.FindPathVisualised(currentNode, endNode);;
        
        List<Node> path = pathfindingVisualiser.FindPath(currentNode, endNode);
        
        foreach (var node in path)
        {
            yield return ColorNode(node);
        }
        
        foreach (var node in path)
        {
            yield return TakeOneStep(node);
        }
        findingPath = null;
    }

    object TakeOneStep(Node node)
    {
        currentCoordinates = node.Coordinates;
        transform.position = currentGrid.GetWorldPosFromCoords(currentCoordinates);
        return new WaitForSeconds(playerStepTime);
    }

    object ColorNode(Node node)
    {
        generator.Visualiser.SetTempSpriteColor(node, playerInfo.FoundPathColor);
        return new WaitForSeconds(pathColoringTime);
    }

    void StartPathfinding()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Node destinationNode = GetDestinationNode();
            if (destinationNode != null)
            {
                if (findingPath != null)
                {
                    StopPreviousPathfinding();
                }
                findingPath = StartCoroutine(FindPath(destinationNode));
            }
        }
    }
    void StopPreviousPathfinding()
    {
        StopCoroutine(findingPath);
        pathfindingVisualiser.ResetNodeTexts();
    }
    Node GetDestinationNode()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        currentGrid.GetGridCoordsFromWorld(mousePos, out int x, out int y);

        Node destinationNode = currentGrid.GetGridType(x, y);
        return destinationNode;
    }
    
    void OnDrawGizmos()
    {
        if (generator == null || playerInfo == null)
        {
            return;
        }
        Node currentNode = currentGrid.GetGridType(currentCoordinates);
        if (currentNode == null) return;
        
        Vector3 playerPos = currentGrid.GetWorldPosFromCoords(currentNode.Coordinates);

        Gizmos.color = playerInfo.PlayerColor;
        Gizmos.DrawSphere(playerPos, playerInfo.PlayerRadius);
    }

    public void ChangeColoringPace(float newValue)
    {
        pathColoringTime = newValue;
    }
    public void ChangeStepPace(float newValue)
    {
        playerStepTime = newValue;
    }
    
    
}


#region StoredForFutureReference COROUTINES ABILITY


// IEnumerator FindPath(Node endNode)
// {
//     if (pathfindingVisualiser ==null)
//     {
//         pathfindingVisualiser = FindObjectOfType<Pathfinding>();
//     }
//
//
//     Node currentNode = currentGrid.GetGridType(currentCoordinates);
//     IEnumerator target = pathfindingVisualiser.FindPathVisualised(currentNode, endNode);
//     yield return target;
//         
//         
//     target = pathfindingVisualiser.FindPath(currentNode, endNode);
//     listOfNodes = new List<Node>();
//     while (target.MoveNext())
//     {
//         if (target.Current?.GetType() == typeof(List<Node>))
//         {
//         
//             listOfNodes = (List<Node>)target.Current;
//             break;
//         }
//     }
//         
//     foreach (var node in listOfNodes)
//     {
//         generator.Visualiser.SetSpriteColor(node,foundPathColor);
//         yield return new WaitForSeconds(colorTime);
//     }
//     foreach (var node in listOfNodes)
//     {
//         currentCoordinates = node.Coordinates;
//         this.transform.position = currentGrid.GetWorldPosFromCoords(currentCoordinates);
//         yield return new WaitForSeconds(playerStepTime);
//     }
//
//     findingPath = null;
// }

#endregion