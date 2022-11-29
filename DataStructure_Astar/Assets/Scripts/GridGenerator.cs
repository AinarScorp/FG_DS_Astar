
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class GridGenerator : MonoBehaviour, ISerializationCallbackReceiver
{
    
    [SerializeField] Color nonWalkableColorSprite = Color.black;
    [SerializeField] Color walkableColorSprite = Color.white;
    [SerializeField] SpriteRenderer square1x1;
    [SerializeField]CustomGrid<Node> nodeGrid;

    
    [SerializeField] Transform parentForTexts;
    [SerializeField] Transform parentForSprites;
    SpriteRenderer[,] spriteArray;

    
    public CustomGrid<Node> NodeGrid => nodeGrid;

    public Transform ParentForTexts => parentForTexts;
    
    #region Serialize stuff



    [SerializeField] List<Package<SpriteRenderer>> storedGridArrayElements;
    

    
    
    public void OnBeforeSerialize()
    {
        if (nodeGrid == null) return;

        storedGridArrayElements = new List<Package<SpriteRenderer>>();
        
        for (int x = 0; x < spriteArray.GetLength(0); x++)
        {
            for (int y = 0; y < spriteArray.GetLength(1); y++)
            {
                storedGridArrayElements.Add(new Package<SpriteRenderer>(x, y, spriteArray[x, y]));
            }
        }
        
   

    }
    
    
    public void OnAfterDeserialize()
    {
        if (nodeGrid == null) return;

        spriteArray = new SpriteRenderer[nodeGrid.Width, nodeGrid.Height];

        foreach(var package in storedGridArrayElements)
        {
            spriteArray[package.Index0, package.Index1] = package.Element;
        }
        
    }
    #endregion

    void Awake()
    {
        DestroyItselfIfGeneratorExists();
    }

    void OnEnable()
    {
        if (nodeGrid == null)
        {
            return;
        }
        foreach (var node in nodeGrid.GridArray)
        {
            MakeNodeNonWalkableVisuals(node);
            node.OnWalkableChanged += MakeNodeNonWalkableVisuals;
        }
    }

    void DestroyItselfIfGeneratorExists()
    {
        GridGenerator[] gridGenerators = FindObjectsOfType<GridGenerator>();

        if (gridGenerators.Length > 1)
        {
            Debug.Log("ss");
            DestroyImmediate(this.gameObject);
        }
    }

    void OnDestroy()
    {
        DeleteCoordsText();
    }

    public void CreateGrid(int width, int height, Vector2 cellSize)
    {
        DeleteCoordsText();
        DeleteSprites();

        nodeGrid = new CustomGrid<Node>(width, height, cellSize,transform.position, (coords) => new Node(coords));

 
        storedGridArrayElements = new List<Package<SpriteRenderer>>();
        spriteArray = new SpriteRenderer[nodeGrid.Width, nodeGrid.Height];

        for (int x = 0; x < spriteArray.GetLength(0); x++)
        {
            for (int y = 0; y < spriteArray.GetLength(1); y++)
            {
                Vector3 position = nodeGrid.GetWorldPosFromCoords(nodeGrid.GridArray[x, y].Coordinates);
                spriteArray[x, y] = Instantiate(square1x1, position, Quaternion.identity, parentForSprites);
                spriteArray[x, y].transform.localScale = new Vector3(nodeGrid.CellSize.x, nodeGrid.CellSize.y, 1);
                spriteArray[x, y].color = walkableColorSprite;
            }
        }
        SceneVisibilityManager.instance.DisablePicking(parentForSprites.gameObject, true);
        
        foreach (var node in nodeGrid.GridArray)
        {
            node.OnWalkableChanged += MakeNodeNonWalkableVisuals;
        }
    }
    public void SetTempSpriteColor(Node node, Color newTempColor)
    {
        node.tempColor = newTempColor;
        spriteArray[node.Coordinates.x, node.Coordinates.y].color = newTempColor;
    }

    public void SetSpriteColor(Node node, Color newColor)
    {
        node.currentColor = newColor;
        spriteArray[node.Coordinates.x, node.Coordinates.y].color = newColor;
    
    }

    public void RestoreColor(Node node)
    {
        spriteArray[node.Coordinates.x, node.Coordinates.y].color = node.currentColor;
    }
    public Color GetSpriteColor(Node node)
    {
        return spriteArray[node.Coordinates.x, node.Coordinates.y].color;
    }

    void MakeNodeNonWalkableVisuals(Node node)
    {
        if (node.IsWalkable)
        {
            SetSpriteColor(node,walkableColorSprite);
        }
        else
        {
            SetSpriteColor(node,nonWalkableColorSprite);
        }
    }
    

    void DeleteSprites()
    {
        foreach (var sprite in spriteArray)
        {
            if (sprite == null) return;
            DestroyImmediate(sprite.gameObject);
        }

        spriteArray = null;
    }
    public void DeleteCoordsText()
    {
        if (!DisplayingCoordinates())
        {
            return;
        }

        foreach (var text in nodeGrid.DebugTexts)
        {
            if (text == null) return;
            DestroyImmediate(text.gameObject);
        }

        nodeGrid.ResetDebugTexts();
    }

    public bool DisplayingCoordinates()
    {
        if (nodeGrid == null) return false;
        return nodeGrid.DisplayingCoordinates;
    }

    //
    // CustomGrid<Node> nodeGrid;
    //
    //
    //
    //
    // public CustomGrid<Node> CustomGrid => nodeGrid;

    // void OnEnable()
    // {
    //
    //     
    //     if (!performCleanUp) return;
    //
    //     CreateCoordsText();
    // }
    //
    // void OnDisable()
    // {
    //     DeleteCoordsText();
    // }



    // void OnDrawGizmos()
    // {
    //     if (nodeGrid == null) return;
    //
    //     Handles.color = lineColor;
    //     nodeGrid.VisualiseGrid(lineThickness);
    //
    //
    //     if (objectForManipulation == null) return;
    //
    //     Gizmos.color = testObjectColor;
    //     Vector3 objectPosition = objectForManipulation.position;
    //     Gizmos.DrawWireSphere(objectPosition, objectRadius);
    //
    //     // nodeGrid.GetGridCoordsFromWorld(objectPosition, out int x, out int y);
    //     // gridPos = new Vector2(x, y);
    //     //
    //     // nodeGrid.SetDebugTextColor(x, y, testObjectColor);
    // }

    //[ContextMenu("Create Coordinates")]
    // void CreateCoordsText()
    // {
    //     if (nodeGrid?.DebugTexts != null && nodeGrid.DebugTexts.Length > 0)
    //     {
    //         DeleteCoordsText();
    //     }
    //
    //     nodeGrid = new CustomGrid<Node>(width, height, cellSize, transform.position, (coords) => new Node(coords));
    //
    //     nodeGrid.DisplayGridCoordinates(coordsColor, coordsFontSize, parentForTexts);
    // }
    //
    // [ContextMenu("Delete Coordinates")]
    // void DeleteCoordsText()
    // {
    //     if (nodeGrid?.DebugTexts == null || nodeGrid.DebugTexts.Length < 1)
    //     {
    //         return;
    //     }
    //
    //     foreach (var text in nodeGrid.DebugTexts)
    //     {
    //         if (text == null) return;
    //         DestroyImmediate(text.gameObject);
    //     }
    //
    //     nodeGrid.ResetDebugTexts();
    // }
}