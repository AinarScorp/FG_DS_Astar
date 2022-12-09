using UnityEngine;
using UnityEditor;
using TMPro;

[ExecuteAlways]
public class GridVisualiser : MonoBehaviour
{
    [SerializeField] Transform parentForTexts;
    [SerializeField] Transform parentForSprites;
    
    [SerializeField] GridGenerator generator;
    [SerializeField] Square1x1 square1x1;
    
    [SerializeField] TwoDimensionalArraySerializer<Square1x1> squareSerializer;

    TextMeshPro[,] coordinatesTexts;
    Square1x1[,] spriteArray => squareSerializer.MultiArray;
    CustomGrid<Node> nodeGrid => generator.NodeGrid;

    void OnEnable()
    {
        SubscribeToTileChange();
    }
    void OnDestroy()
    {
        DeleteCoordsText();
    }
    
    public void VisualiseNewGrid()
    {
        DeleteCoordsText();
        DeleteSprites();
        CreateSprites();
        SubscribeToTileChange();
    }

    void SubscribeToTileChange()
    {
        if (nodeGrid == null)
        {
            return;
        }
        foreach (var node in nodeGrid.GridArray)
        {
            node.OnTileTypeChanged += SetTileColor;
            SetTileColor(node);
        }
    }

    #region 1x1 Sprites

    void CreateSprites()
    {
        Square1x1[,] spriteArrayCopy = new Square1x1[nodeGrid.Width, nodeGrid.Height];

        for (int x = 0; x < spriteArrayCopy.GetLength(0); x++)
        {
            for (int y = 0; y < spriteArrayCopy.GetLength(1); y++)
            {
                Vector3 position = nodeGrid.GetWorldPosFromCoords(nodeGrid.GridArray[x, y].Coordinates);
                spriteArrayCopy[x, y] = Instantiate(square1x1, position, Quaternion.identity, parentForSprites);
                spriteArrayCopy[x, y].transform.localScale = new Vector3(nodeGrid.CellSize.x, nodeGrid.CellSize.y, 1);
            }
        }

        squareSerializer = new TwoDimensionalArraySerializer<Square1x1>(spriteArrayCopy);

        SceneVisibilityManager.instance.DisablePicking(parentForSprites.gameObject, true);
    }
    
    void DeleteSprites()
    {
        if (spriteArray == null)
        {
            return;
        }
        foreach (var sprite in spriteArray)
        {
            if (sprite == null) return;
            DestroyImmediate(sprite.gameObject);
        }

        squareSerializer = null;
    }

    #endregion
       
    #region Changing colors for debugging purposes
    SpriteRenderer GetSpriteRendererByNode(Node node, bool chooseChild)
    {
        Square1x1 square = spriteArray[node.Coordinates.x, node.Coordinates.y];
        return chooseChild ? square.ChildSprite : square.ThisSprite;
    }
    public void SetTempSpriteColor(Node node, Color newTempColor,bool setChildSprite = true)
    {
        GetSpriteRendererByNode(node,setChildSprite).color = newTempColor;
    }
    public void RestoreColor(Node node, bool childSprite = true)
    {
        GetSpriteRendererByNode(node,childSprite).color = node.Visualiser.CurrentColor;
    }
    public void SetSpriteColor(Node node, Color newColor, bool setChildSprite = true)
    {
        node.Visualiser.SetCurrentColor(newColor);
        GetSpriteRendererByNode(node,setChildSprite).color = newColor;
    }

    void SetTileColor(Node node)
    {
        SetSpriteColor(node, node.TileColor, false);
    }

    public void ResetSpriteColors()
    {
        foreach (var sprite in spriteArray)
        {
            sprite.ChildSprite.color = sprite.DefaultChildColor;
        }
    }
    #endregion

    #region Coordinates

    public void DisplayGridCoordinates(Color textColor, float textFontSize = 8)
    {
        int width = nodeGrid.Width;
        int height = nodeGrid.Height;
        
        coordinatesTexts = new TextMeshPro[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                coordinatesTexts[x, y] = CreateCoordinatesObject(textColor, textFontSize, x, y);
            }
        }
        if (parentForTexts != null)
        {
            SceneVisibilityManager.instance.DisablePicking(parentForTexts.gameObject, true);
        }
    }

    public bool DisplayingCoordinates()
    {
        if (nodeGrid == null) return false;
        return coordinatesTexts != null && coordinatesTexts.Length > 0;;
    }

    public void DeleteCoordsText()
    {
        if (!DisplayingCoordinates())
        {
            return;
        }

        foreach (var text in coordinatesTexts)
        {
            if (text == null) return;
            DestroyImmediate(text.gameObject);
        }

        coordinatesTexts = null;
    }

    TextMeshPro CreateCoordinatesObject(Color textColor, float textFontSize, int x, int y)
    {
        float cellSizeX = nodeGrid.CellSize.x;
        float cellSizeY = nodeGrid.CellSize.y;
        Vector3 originPosition = transform.position;

        string textName = $"{x};{y}";
        Vector3 textPos = new Vector3(x * cellSizeX + cellSizeX * 0.5f, y * cellSizeY + cellSizeY * 0.5f) + originPosition;
        return Visualisation.CreateText(textName, textName, textPos, textColor, textFontSize, parentForTexts);
    }
    #endregion
    

}
