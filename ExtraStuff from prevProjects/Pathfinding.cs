using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Strategy.Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        private static Pathfinding instance;
        const int MOVE_STRAIGT_COST = 10;
        const int MOVE_DIAGONAL_COST = 14;

        [Header("Wiring")]
        [SerializeField] Grid tileGrid;
        [SerializeField] Tilemap tileMap;


        [Header("Settings")]
        [SerializeField] bool showDebug = true;
        [SerializeField] bool showGridDebug = true;

        [SerializeField] int width = 10, height = 10;

        Vector2 cellSize;
        IsometricGrid<PathNode> isoGrid;
        List<PathNode> openList;
        List<PathNode> closedList;

        public IsometricGrid<PathNode> IsoGrid { get => isoGrid; }
        public int Move_Straight_Cost { get => MOVE_STRAIGT_COST; }
        public static Pathfinding Instance { get => instance; }
        public Grid TileGrid { get => tileGrid; }
        public Tilemap TileMap { get => tileMap;  }

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this.gameObject);

            if (tileGrid == null)
                tileGrid = FindObjectOfType<Grid>();


            cellSize = new Vector2(tileGrid.cellSize.x, tileGrid.cellSize.y) * 10;
            BoundsInt bounds = tileMap.cellBounds;


            width = Mathf.Abs(bounds.max.x - bounds.min.x);
            height = Mathf.Abs(bounds.max.y -bounds.min.y);
            Vector3 origin = tileMap.CellToWorld(new Vector3Int(bounds.min.x, bounds.min.y, bounds.min.z));
            isoGrid = new IsometricGrid<PathNode>(width, height, cellSize, origin, (IsometricGrid<PathNode> Grid, Coordinates coords) => new PathNode(Grid, coords, this.transform), showDebug);

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (int z = bounds.min.z; z < bounds.max.z; z++)
                    {
                        var tile = tileMap.GetTile(new Vector3Int(x, y, z));
                        Vector3 place = tileMap.CellToWorld(new Vector3Int(x, y, z));
                        Coordinates coords = isoGrid.GetCoords(place);

                        if (showGridDebug)
                        {
                            isoGrid.CreateText(new Vector3(place.x, place.y + cellSize.y * 0.5f, place.z), "tileText", $"{x} , {y}, {z}");
                        }
                        if (!tileMap.HasTile(new Vector3Int(x, y, z)))
                        {

                            GetNode(coords.x, coords.y).BlockPath();
                        }
                        else
                        {
                            Vector3Int tileCoords = new Vector3Int(x, y, z);
                            GetNode(coords).AssignTile(tileMap.GetTile(tileCoords), tileCoords);
                        }
                    }
                }

            }

        }



        public List<PathNode> FindPath(Coordinates startNodeCoords, Coordinates endNodeCoords)
        {
            PathNode startNode = GetNode(startNodeCoords);
            PathNode endNode = GetNode(endNodeCoords);
            if (startNode == null || endNode == null)
            {
                print("INVALID PATH");
                return null;
            }

            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();
            for (int x = 0; x < isoGrid.GridArray.GetLength(0); x++) //code monkey has it different
            {
                for (int y = 0; y < isoGrid.GridArray.GetLength(1); y++)
                {
                    PathNode pathNode = isoGrid.GetGridType(new Coordinates(x, y));
                    pathNode.ResetNode();
                }
            }

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    //found Path
                    return CalculatePath(endNode);
                }
  
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.IsWalkable)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }
                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();
                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }
            //no available path
            print("No path available");

            return null;

        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;
            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;

            }
            path.Reverse();
            return path;

        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostPathNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].FCost < lowestFCostPathNode.FCost)
                    lowestFCostPathNode = pathNodeList[i];
            }
            return lowestFCostPathNode;
        }

        public int CalculateDistanceCost(PathNode startNode, PathNode endNode)
        {
            if (startNode == null || endNode == null)
                print("start/end node is  null check CalculateDistanceCost method");

            int xDistance = Mathf.Abs(startNode.Coordinates.x - endNode.Coordinates.x);
            int yDistance = Mathf.Abs(startNode.Coordinates.y - endNode.Coordinates.y);
            int remaining = Mathf.Abs(xDistance - yDistance);

            //print($"Start Node Coords: {startNode.Coordinates.x};{startNode.Coordinates.y}\n" +
            //    $"End Node Coords: {endNode.Coordinates.x};{endNode.Coordinates.y}\n" +
            //    $"Calculated Cost: {MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGT_COST * remaining}");
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGT_COST * remaining;

        }
        List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();
            Coordinates currentNodeCoords = currentNode.Coordinates;
            bool thereIsANodeUpRight = currentNodeCoords.x + 1 < isoGrid.Width;
            bool thereIsANodeUpLeft = currentNodeCoords.y + 1 < isoGrid.Height;
            bool thereIsANodeDownRight = currentNodeCoords.y - 1 >= 0;
            bool thereIsANodeDownLeft = currentNodeCoords.x - 1 >= 0;

            if (thereIsANodeUpRight)
            {
                neighbourList.Add(GetNode(currentNodeCoords.x + 1, currentNodeCoords.y));
                if (thereIsANodeDownRight)
                {
                    neighbourList.Add(GetNode(currentNodeCoords.x + 1, currentNodeCoords.y - 1));
                }
                if (thereIsANodeUpLeft)
                {
                    neighbourList.Add(GetNode(currentNodeCoords.x + 1, currentNodeCoords.y + 1));
                }
            }
            if (thereIsANodeDownLeft)
            {
                neighbourList.Add(GetNode(currentNodeCoords.x - 1, currentNodeCoords.y));
                if (thereIsANodeUpLeft)
                {
                    neighbourList.Add(GetNode(currentNodeCoords.x - 1, currentNodeCoords.y + 1));
                }
                if (thereIsANodeDownRight)
                {
                    neighbourList.Add(GetNode(currentNodeCoords.x - 1, currentNodeCoords.y - 1));
                }
            }
            if (thereIsANodeUpLeft)
            {
                neighbourList.Add(GetNode(currentNodeCoords.x, currentNodeCoords.y + 1));
            }
            if (thereIsANodeDownRight)
            {
                neighbourList.Add(GetNode(currentNodeCoords.x, currentNodeCoords.y - 1));
            }
            return neighbourList;
        }
        public PathNode GetNodeFromWorldPos(Vector3 worldPosition)
        {
            Coordinates coords = isoGrid.GetCoords(worldPosition);
            return GetNode(coords);
        }
        public PathNode GetNode(Coordinates coordinates) => GetNode(coordinates.x, coordinates.y);
        public PathNode GetNode(int x, int y) => isoGrid.GetGridType(new Coordinates(x, y));
    }

}
