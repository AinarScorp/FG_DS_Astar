using UnityEngine;

[CreateAssetMenu(fileName = "Player Info", menuName = "ScriptableObjects/Player Info", order = 4)]

public class PlayerInfo : ScriptableObject
{
    [Header("Player Editor window Info")]
    [SerializeField] float playerRadius = 2;
    [SerializeField] Color playerColor = Color.green;
    [Header("Pathfinding Info")]
    [SerializeField] float playerStepTime = 0.5f;
    [SerializeField] float pathColoringTime = 2;
    [SerializeField] Color foundPathColor = Color.cyan;

    public float PlayerRadius => playerRadius;
    public Color PlayerColor => playerColor;
    
    public float PlayerStepTime => playerStepTime;
    public float PathColoringTime => pathColoringTime;
    public Color FoundPathColor => foundPathColor;
}
