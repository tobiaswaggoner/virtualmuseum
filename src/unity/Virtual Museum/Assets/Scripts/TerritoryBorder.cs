using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryBorder
{
    public TerritoryBorder (int startTime, Color Color){
        this.startTime = startTime;
        this.Color = Color;
    }
    public List<Vector3> Points = new List<Vector3>();
    public int startTime;
    public Color Color; 
}
