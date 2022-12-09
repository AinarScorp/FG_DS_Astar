using System;
using System.IO;
using UnityEngine;


[Serializable]
public class MapControllerData
{
    const string FILE_NAME = "Grid generator Data.json";

    [SerializeField] int width, height;
    
    [SerializeField] Vector2 cellSize;


    [SerializeField] FloatColorInfo gridLineInfo;
    [SerializeField] FloatColorInfo coordinatesGizmosInfo;

    [SerializeField] Vector2Int playerPosition;
    

    public MapControllerData() { }
    public MapControllerData(int width, int height, Vector2 cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
    }
    public void SaveCoordinatesInfo(float coordinatesFontSize, Color coordinatedColor)
    {
        coordinatesGizmosInfo = new FloatColorInfo(coordinatesFontSize, coordinatedColor);
    }

    public void SaveGridLineInfo(float lineThickness, Color lineColor)
    {
        gridLineInfo = new FloatColorInfo(lineThickness,lineColor);
    }


    public void SavePlayerInfo(Vector2Int playerPosition)
    {
        this.playerPosition = playerPosition;
    }
    public void Save()
    {
        string jsonGridData = JsonUtility.ToJson(this);
        File.WriteAllText(Application.dataPath + FILE_NAME, jsonGridData);
    }

    public MapControllerData Load()
    {
        string jsonGridData = File.ReadAllText(Application.dataPath + FILE_NAME);
        return JsonUtility.FromJson<MapControllerData>(jsonGridData);
    }

    public void LoadGridData(ref int width,ref int height, ref Vector2 cellSize)
    {
        width = this.width;
        height = this.height;
        cellSize = this.cellSize;
    }

    public void LoadGridLineInfo(ref float lineThickness, ref Color lineColor)
    {
        lineThickness = gridLineInfo.FloatValue;
        lineColor = gridLineInfo.ColorValue;
    }
    public void LoadCoordinatesInfo(ref float coordinatesFontSize, ref Color coordinatedColor)
    {
        coordinatesFontSize = coordinatesGizmosInfo.FloatValue;
        coordinatedColor = coordinatesGizmosInfo.ColorValue;
    }
    public void LoadPlayerInfo(ref Vector2Int playerPosition)
    {
        playerPosition = this.playerPosition;
    }


    [Serializable]
    public struct FloatColorInfo
    {
        [SerializeField] float floatValue;
        [SerializeField] Color colorValue;
        public FloatColorInfo(float floatValue, Color colorValue)
        {
            this.floatValue = floatValue;
            this.colorValue = colorValue;
        }

        public float FloatValue => floatValue;
        public Color ColorValue => colorValue;
    }

}
