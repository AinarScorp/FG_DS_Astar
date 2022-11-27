using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class BubbleNumber : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] int number;

    [SerializeField, HideInInspector] Color defaultColor;
    public int Number => number;
    void OnEnable()
    {
        SceneVisibilityManager.instance.DisablePicking(text.gameObject, true);
        defaultColor = spriteRenderer.color;
    }

    void UpdateName()
    {
        string stringName = number.ToString();
        
        gameObject.name = stringName;
        text.text = stringName;
    }

    public void SetName(int newNumber)
    {
        number = newNumber;
        UpdateName();
        
    }

    public void SetColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }

    public void ResetColor()
    {
        spriteRenderer.color = defaultColor;
    }

}
