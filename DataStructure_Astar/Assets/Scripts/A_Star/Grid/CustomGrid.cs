using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


[Serializable]
public class CustomGrid<TGridType>
{
    [SerializeField] int width, height;
    [SerializeField] Vector2 cellSize;
    [SerializeField] Vector3 originPosition;
    
    [SerializeField] TwoDimensionalArraySerializer<TGridType> TgridTypeSerializer;
    TGridType[,] gridArray => TgridTypeSerializer.MultiArray;
    
    public int Width => width;
    public int Height => height;
    public Vector2 CellSize => cellSize;
    public TGridType[,] GridArray => gridArray;



    public CustomGrid(int width, int height, Vector2 cellSize, Vector3 originPosition, Func<Coordinates, TGridType> createGridType)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        
        TGridType[,] gridArray = new TGridType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = createGridType(new Coordinates(x, y));
            }
        }
        TgridTypeSerializer = new TwoDimensionalArraySerializer<TGridType>(gridArray);
    }

    public void GetGridCoordsFromWorld(Vector3 worldPosition, out int x, out int y)
    {
        Vector2 objectPos = worldPosition - originPosition;
        x = Mathf.FloorToInt((objectPos.x) / cellSize.x);
        y = Mathf.FloorToInt((objectPos.y) / cellSize.y);

    }

    public Vector3 GetWorldPosFromCoords(Coordinates coordinates,bool centered = true)
    {
        return GetWorldPosFromCoords(coordinates.x, coordinates.y,centered);
    }

    public Vector3 GetWorldPosFromCoords(int x, int y, bool centered = true)
    {
        float centerOffset = centered ? 0.5f : 0;
        float xPos = x * cellSize.x + cellSize.x * centerOffset;
        float yPos = y * cellSize.y +cellSize.y * centerOffset;
        return new Vector3(xPos, yPos) + originPosition;
    }

    public TGridType GetGridType(Coordinates coordinates)
    {
        return GetGridType(coordinates.x, coordinates.y);
    }
    
    public TGridType GetGridType(int x, int y)
    {
        if (OutOfArray(x,y))
        {
            return default;
        }
        
        return gridArray[x, y];
    }
    public TGridType SetGridType(int x, int y, TGridType setValue)
    {
        if (OutOfArray(x,y))
        {
            return default;
        }
        return gridArray[x, y] = setValue;
    }

    bool OutOfArray(int x, int y)
    {
        return x >= width || x < 0 || y >= height || y < 0;
    }
    public void VisualiseGrid(Color gridColor, float lineThickness)
    {
        Handles.color = gridColor;
        for (int x = 0; x < width; x++)
        {

            Vector3 startLinePos = new Vector3(x * cellSize.x, 0) + originPosition;
            Vector3 endLineUp = new Vector3(x * cellSize.x, height * cellSize.y) + originPosition;
            Handles.DrawLine(startLinePos, endLineUp, lineThickness);
        }

        for (int y = 0; y < height; y++)
        {
            Vector3 startLinePos = new Vector3(0, y * cellSize.y) + originPosition;
            Vector3 endLineRight = new Vector3(width * cellSize.x, y * cellSize.y) + originPosition;
            Handles.DrawLine(startLinePos, endLineRight, lineThickness);
        }

        Vector3 finishingLineCorner = new Vector3(width * cellSize.x, height * cellSize.y) + originPosition;
        Vector3 finishingLineTop = new Vector3(0, height * cellSize.y) + originPosition;
        Vector3 finishingLineRight = new Vector3(width * cellSize.x, 0) + originPosition;

        Handles.DrawLine(finishingLineTop, finishingLineCorner, lineThickness);
        Handles.DrawLine(finishingLineRight, finishingLineCorner, lineThickness);


    }


}

