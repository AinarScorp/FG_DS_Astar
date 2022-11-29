using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

public class Visualisation
{
    public static TextMeshPro CreateText(string name, string text,Vector3 position,Color textColor, float textFontSize = 8, Transform parentForTexts = null, bool disablePicking = false)
    {
        GameObject newText = new GameObject(name, typeof(TextMeshPro));
        newText.transform.position = position;
        TextMeshPro textObject = newText.GetComponent<TextMeshPro>();
        textObject.text = text;
        textObject.alignment = TextAlignmentOptions.Center;
        textObject.color = textColor;
        textObject.fontSize = textFontSize;
        if (parentForTexts != null)
        {
            textObject.transform.SetParent(parentForTexts);
        }
        
        if (disablePicking)
        {
            SceneVisibilityManager.instance.DisablePicking(textObject.gameObject, true);
        }

        return textObject;
    }
}