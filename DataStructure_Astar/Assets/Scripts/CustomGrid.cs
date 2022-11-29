using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


[Serializable]
public class CustomGrid<TGridType> : ISerializationCallbackReceiver
{
    [SerializeField] int width, height;
    [SerializeField] Vector2 cellSize;
    [SerializeField] Vector3 originPosition;


    TGridType[,] gridArray;


    TextMeshPro[,] debugTexts;

    public TextMeshPro[,] DebugTexts => debugTexts;
    public bool DisplayingCoordinates => debugTexts != null && debugTexts.Length > 0;

    public TGridType[,] GridArray => gridArray;

    public int Width => width;

    public int Height => height;

    public Vector2 CellSize => cellSize;

    #region Serialize stuff



    [SerializeField] List<Package<TGridType>> storedGridArrayElements;
    

    
    
    public void OnBeforeSerialize()
    {
        storedGridArrayElements = new List<Package<TGridType>>();
        
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                storedGridArrayElements.Add(new Package<TGridType>(x, y, gridArray[x, y]));
            }
        }
    }
    
    
    public void OnAfterDeserialize()
    {
        gridArray = new TGridType[width, height];

        foreach(var package in storedGridArrayElements)
        {
            gridArray[package.Index0, package.Index1] = package.Element;
        }
    }
    #endregion

    public CustomGrid(int width, int height, Vector2 cellSize, Vector3 originPosition, Func<Coordinates, TGridType> createGridType)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        
        gridArray = new TGridType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = createGridType(new Coordinates(x, y));
            }
        }

    }


    public void DisplayGridCoordinates(Color textColor, float textFontSize = 8, Transform parentForTexts = null)
    {
        debugTexts = new TextMeshPro[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newText = new GameObject($"{x};{y}", typeof(TextMeshPro));
                newText.transform.position = new Vector3(x * cellSize.x + cellSize.x * 0.5f, y * cellSize.y + cellSize.y * 0.5f) + originPosition;
                TextMeshPro textMeshPro = newText.GetComponent<TextMeshPro>();
                textMeshPro.text = $"{x};{y}";
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.color = textColor;
                textMeshPro.fontSize = textFontSize;
                if (parentForTexts != null)
                {
        
                    textMeshPro.transform.SetParent(parentForTexts);
                }
        
                debugTexts[x, y] = textMeshPro;
        
            }
        }

        if (parentForTexts != null)
        {
            SceneVisibilityManager.instance.DisablePicking(parentForTexts.gameObject, true);
        }

    }

    public void ResetDebugTexts()
    {
        debugTexts = null;
    }

    public void VisualiseGrid(Color gridColor, float lineThickness)
    {
        Handles.color = gridColor;

        // for (int x = 0; x < width; x++)
        // {
        //     for (int y = 0; y < height; y++)
        //     {
        //         Vector3 startLinePos = new Vector3(x * cellSize.x, y * cellSize.y) + originPosition;
        //         Vector3 endLineUp = new Vector3(x * cellSize.x, (y + 1) * cellSize.y) + originPosition;
        //         Vector3 endLineRight = new Vector3((x + 1) * cellSize.x, y * cellSize.y) + originPosition;
        //
        //         Handles.DrawLine(startLinePos, endLineRight, lineThickness);
        //         Handles.DrawLine(startLinePos, endLineUp, lineThickness);
        //
        //     }
        // }

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


    
    public void SetDebugTextColor(int x, int y, Color newColor)
    {
        if (debugTexts == null) return;

        if (OutOfArray(x,y)) { return; }

        debugTexts[x, y].color = newColor;

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


}
[Serializable]
struct Package<TElement>
{
    public int Index0;
    public int Index1;
    public TElement Element;
    public Package(int idx0, int idx1, TElement element)
    {
        Index0 = idx0;
        Index1 = idx1;
        Element = element;
    }
}
