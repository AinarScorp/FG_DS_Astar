using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Coordinates
{
    public int x;
    public int y;
    public Coordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Coordinates(Vector2Int vectorCoords)
    {
        this.x = vectorCoords.x;
        this.y = vectorCoords.y;
    }
}
