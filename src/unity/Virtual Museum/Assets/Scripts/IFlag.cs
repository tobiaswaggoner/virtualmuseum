using System;
using UnityEngine;
///<summary>
///The standard Interface for a flag
///A flag is a point of interest on a given map. It is used for displaying information through text and visual animation
/// </summary>
public interface IFlag
{
    int startTime {get; set;}
    Transform transform {get; set;}
    Vector3 position {get; set;}
    GameObject flagVisualTextComponent {get; set;}
    GameObject flagVisualIndicator {get; set;}
    String header {get; set;}
    String info {get; set;}

    //void GetFlagComponents();
    //void DisplayInformation();
    //void StopDisplayingInformation();
    //void CheckForTimeAdvance(float newTime);
}
