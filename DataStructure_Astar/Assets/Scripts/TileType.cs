using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Tile Type", menuName = "ScriptableObjects/Tile Type", order = 2)]
public class TileType : ScriptableObject
{
    [SerializeField] Color typeColor = Color.white;
    [SerializeField] bool isWalkable = true;
    [SerializeField] int walkingCost = 1;


    public Color TypeColor => typeColor;

    public bool IsWalkable => isWalkable;

    public int WalkingCost => walkingCost;
}
