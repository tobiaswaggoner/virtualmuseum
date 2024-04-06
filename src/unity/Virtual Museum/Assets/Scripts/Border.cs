using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Border class in the structure of a connected list, with each Border being part of a chain of borders,
///representing the development of a Territory across a time span
///Each Border has defined edges and values for next and last Border as well as the start and end Border of the chain
///Allows for comparison and animation of territory size
public class Border
{
    public Border startBorder;
    public Border endBorder;
    public Border nextBorder;
    public Border lastBorder;

    public List<Vector3> edges = new List<Vector3>();
    public int startingTime;
    public int endingTime;
    
    public Border(List<Vector3> edges, int startingTime, int endingTime){
        this.edges = edges;
        this.startingTime = startingTime;
        this.endingTime = endingTime;
    }
}
