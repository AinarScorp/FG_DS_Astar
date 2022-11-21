using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "Generator scriptable", menuName = "ScriptableObjects/Generator scriptable", order = 1)]
public class GridGeneratorScriptableObject : ScriptableObject
{
    [SerializeField] GridGenerator gridGeneratorPrefab;
    [Header("Mouse Pointer")]
    [SerializeField] float pointerRadius = 3;
    [SerializeField] Color pointerColor = Color.cyan;
    [SerializeField] float pointerThickness = 2;


    public float PointerRadius => pointerRadius;

    public Color PointerColor => pointerColor;

    public GridGenerator GridGeneratorPrefab => gridGeneratorPrefab;

    public void VisualisePointer(Vector3 mousePosition, Vector3 normal)
    {
        Handles.color = pointerColor;

        Handles.DrawWireDisc(mousePosition,normal, pointerRadius,pointerThickness);
        Vector3 leftPoint = mousePosition;
        leftPoint.x -= pointerRadius;
        Vector3 rightPoint = mousePosition;
        rightPoint.x += pointerRadius;       
        Vector3 upperPoint = mousePosition;
        upperPoint.y += pointerRadius;
        Vector3 bottomPoint = mousePosition;
        bottomPoint.y -= pointerRadius;
        

        Handles.DrawLine(leftPoint, rightPoint,pointerThickness);
        Handles.DrawLine(upperPoint, bottomPoint,pointerThickness);


    }

}
