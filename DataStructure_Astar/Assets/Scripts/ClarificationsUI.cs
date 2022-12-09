using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClarificationsUI : MonoBehaviour
{
    [SerializeField] TileType[] tileTypes;
    [SerializeField] VerticalLayoutGroup bigSquaresExplanation;
    [SerializeField] Vector2 squareSize = new Vector2(100, 100);
    [SerializeField] Image squarePrefab;
    [SerializeField] string walkingCostText = "Walking Cost:";


    [ContextMenu("CreateBigSquares")]
    void CreateBigSquares()
    {
        foreach (var tileType in tileTypes)
        {
            Image square = Instantiate(squarePrefab, bigSquaresExplanation.transform);
            square.rectTransform.sizeDelta = squareSize;
            TextMeshProUGUI squareText = square.GetComponentInChildren<TextMeshProUGUI>();

            square.color = tileType.TypeColor;
            
            string text = $"{tileType.name}\n";
            text += tileType.IsWalkable ? $"{walkingCostText} : {tileType.WalkingCost}" : "Non-walkable";
            squareText.text = text;
        }
    }
    
}
