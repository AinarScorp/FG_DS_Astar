using UnityEngine;
using System.Collections.Generic;


[ExecuteAlways]
public class GridGenerator : MonoBehaviour
{
    [SerializeField] CustomGrid<Node> nodeGrid;
    [SerializeField] TileType defaultTileType;
    [SerializeField] GridVisualiser visualiser;
    
    public CustomGrid<Node> NodeGrid => nodeGrid;
    public GridVisualiser Visualiser => visualiser;
    
    public void CreateGrid(int width, int height, Vector2 cellSize)
    {
        nodeGrid = new CustomGrid<Node>(width, height, cellSize,transform.position, (coords) => new Node(coords, defaultTileType));

        visualiser.VisualiseNewGrid();

    }
    public List<Node> GetNeighbourList(Node node)
    {
        Coordinates coordinates = node.Coordinates;

        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++) 
        {
            for (int y = -1; y <= 1; y++) 
            {
                if (x == 0 && y == 0) 
                {
                    continue; //Same Node
                }
                if (NodeExistsOnGrid(coordinates.x + x, coordinates.y + y)) 
                {
                    neighbors.Add(NodeGrid.GetGridType(coordinates.x+x,coordinates.y+y));
                }
            }
        }
        return neighbors;
    }

    bool NodeExistsOnGrid(int x, int y) 
    {
        return x >= 0 && y >= 0 && x < NodeGrid.Width && y < NodeGrid.Height;
    }
    
}