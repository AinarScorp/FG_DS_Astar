using UnityEngine;

[CreateAssetMenu(fileName = "Pathfinding Info", menuName = "ScriptableObjects/Pathfinding Info", order = 3)]
public class PathfindingInfo : ScriptableObject
{
    [Header("Visualiser")]
    [SerializeField] string defaultText = "---";
    [SerializeField] Color currentTileColor = Color.red;
    [SerializeField] Color wallTileColor = Color.grey;
    [SerializeField] Color neighbourTileColor = Color.yellow;
    
    [Header("Visualiser Arrow")]
    [SerializeField][Range(0,360)] float arrowAngleDeg = 30;
    [SerializeField][Range(0,20)] float arrowThickness = 3;
    [SerializeField][Range(0,20)] float arrowHeadLength = 5;
    [SerializeField] Color pointingArrowColor = Color.white;
    [Header("Visualiser Dot")]
    [SerializeField][Range(0,20)] float dotRadius = 3;
    [SerializeField] Color dotColor = Color.black;

    
    
    public string DefaultText => defaultText;
    public Color CurrentTileColor => currentTileColor;
    public Color WallTileColor => wallTileColor;
    public Color NeighbourTileColor => neighbourTileColor;

    public Color PointingArrowColor => pointingArrowColor;
    public float ArrowThickness => arrowThickness;
    public float ArrowAngleRad => arrowAngleDeg * Mathf.Deg2Rad;

    public Color DotColor => dotColor;

    public float DotRadius => dotRadius;

    public float ArrowHeadLength => arrowHeadLength;
}
