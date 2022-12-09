using System;
using UnityEngine;
using TMPro;

[Serializable]
public class NodeVisualiser
{
    [SerializeField] TextMeshPro debugText;
    
    Color currentColor;
    public Color CurrentColor => currentColor;
    public TextMeshPro DebugText => debugText;
    
    #region Debug Texts
    
    public void AssignDebugText(TextMeshPro textCube,string text, Color textColor)
    {
        textCube.text = text;
        textCube.color = textColor;
        debugText = textCube;
        
    }

    public void VisualiseCosts(int fromStartCost, int estimateCost)
    {
        int combinedCost = fromStartCost + estimateCost;
        string textCosts = $"S:{fromStartCost} H:{estimateCost} C:{combinedCost}";
        ChangeText(textCosts);

    }
    public void ChangeText(string newText)
    {
        if (debugText != null)
        {
            debugText.text = newText;
        }
    }

    #endregion
    
    public void SetCurrentColor(Color newCurrentColor)
    {
        currentColor = newCurrentColor;
    }
}
