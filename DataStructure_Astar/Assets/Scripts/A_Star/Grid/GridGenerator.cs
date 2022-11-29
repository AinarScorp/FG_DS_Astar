
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class GridGenerator : MonoBehaviour, ISerializationCallbackReceiver
{
    
    [SerializeField] Square1x1 square1x1;
    [SerializeField] CustomGrid<Node> nodeGrid;

    
    [SerializeField] Transform parentForTexts;
    [SerializeField] Transform parentForSprites;
    Square1x1[,] spriteArray;

    
    TextMeshPro[,] debugTexts;

    [SerializeField,HideInInspector] TileType defaultTileType;
    public CustomGrid<Node> NodeGrid => nodeGrid;

    
    #region Serialize stuff
    
    [SerializeField, HideInInspector] List<DataToSerialise<Square1x1>> storedGridArrayElements;
    
    
    public void OnBeforeSerialize()
    {
        if (nodeGrid == null) return;

        storedGridArrayElements = new List<DataToSerialise<Square1x1>>();
        
        for (int x = 0; x < spriteArray.GetLength(0); x++)
        {
            for (int y = 0; y < spriteArray.GetLength(1); y++)
            {
                storedGridArrayElements.Add(new DataToSerialise<Square1x1>(x, y, spriteArray[x, y]));
            }
        }
        
   

    }
    
    
    public void OnAfterDeserialize()
    {
        if (nodeGrid == null) return;

        spriteArray = new Square1x1[nodeGrid.Width, nodeGrid.Height];

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

        if (defaultTileType == null)
        {
            defaultTileType = Resources.Load<TileType>("Tiles/Road");
        }
        foreach (var node in nodeGrid.GridArray)
        {
            SetTileColor(node);
            node.OnTileTypeChanged += SetTileColor;
        }
    }

    
    void DestroyItselfIfGeneratorExists()
    {
        GridGenerator[] gridGenerators = FindObjectsOfType<GridGenerator>();

        if (gridGenerators.Length > 1)
        {
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

        nodeGrid = new CustomGrid<Node>(width, height, cellSize,transform.position, (coords) => new Node(coords, defaultTileType));
        
        CreateSquares();

        SubscribeToTileChange();
    }

    void SubscribeToTileChange()
    {
        foreach (var node in nodeGrid.GridArray)
        {
            node.OnTileTypeChanged += SetTileColor;
            SetTileColor(node);
        }
    }

    void CreateSquares()
    {
        storedGridArrayElements = new List<DataToSerialise<Square1x1>>();
        spriteArray = new Square1x1[nodeGrid.Width, nodeGrid.Height];

        for (int x = 0; x < spriteArray.GetLength(0); x++)
        {
            for (int y = 0; y < spriteArray.GetLength(1); y++)
            {
                Vector3 position = nodeGrid.GetWorldPosFromCoords(nodeGrid.GridArray[x, y].Coordinates);
                spriteArray[x, y] = Instantiate(square1x1, position, Quaternion.identity, parentForSprites);
                spriteArray[x, y].transform.localScale = new Vector3(nodeGrid.CellSize.x, nodeGrid.CellSize.y, 1);
            }
        }

        SceneVisibilityManager.instance.DisablePicking(parentForSprites.gameObject, true);
    }

    public void SetTempSpriteColor(Node node, Color newTempColor,bool setChildSprite = true)
    {
        node.tempColor = newTempColor;
        SpriteRenderer spriteRenderer = DetermineSpriteRenderer(node,setChildSprite);
        spriteRenderer.color = newTempColor;
    }

    public void SetSpriteColor(Node node, Color newColor, bool setChildSprite = true)
    {
        SpriteRenderer spriteRenderer = DetermineSpriteRenderer(node,setChildSprite);
        node.currentColor = newColor;
        spriteRenderer.color = newColor;
    
    }

    SpriteRenderer DetermineSpriteRenderer(Node node, bool chooseChild)
    {
        Square1x1 square = spriteArray[node.Coordinates.x, node.Coordinates.y];
        return chooseChild ? square.ChildSprite : square.ThisSprite;
    }
    public void RestoreColor(Node node, bool childSprite = true)
    {
        SpriteRenderer spriteRenderer = DetermineSpriteRenderer(node,childSprite);

        spriteRenderer.color = node.currentColor;
    }
    public Color GetSpriteColor(Node node, bool childSprite = true)
    {
        SpriteRenderer spriteRenderer = DetermineSpriteRenderer(node,childSprite);

        return spriteRenderer.color;
    }

    void SetTileColor(Node node)
    {
        SetSpriteColor(node,node.TileColor,false);
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

        foreach (var text in debugTexts)
        {
            if (text == null) return;
            DestroyImmediate(text.gameObject);
        }

        debugTexts = null;
    }

    public bool DisplayingCoordinates()
    {
        if (nodeGrid == null) return false;
        return debugTexts != null && debugTexts.Length > 0;;
    }

    
    public void DisplayGridCoordinates(Color textColor, float textFontSize = 8)
    {
        int width = nodeGrid.Width;
        int height = nodeGrid.Height;
        Vector2 cellSize = nodeGrid.CellSize;
        Vector3 originPosition = transform.position;
        
        debugTexts = new TextMeshPro[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                string textName = $"{x};{y}";
                Vector3 textPos = new Vector3(x * cellSize.x + cellSize.x * 0.5f, y * cellSize.y + cellSize.y * 0.5f) + originPosition;
                TextMeshPro textObject = Visualisation.CreateText(textName, textName, textPos, textColor, textFontSize, parentForTexts);
                debugTexts[x, y] = textObject;
            }
        }
        if (parentForTexts != null)
        {
            SceneVisibilityManager.instance.DisablePicking(parentForTexts.gameObject, true);
        }

    }
    
}