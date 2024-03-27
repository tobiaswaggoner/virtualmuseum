using System.Collections;
using System.Collections.Generic;
using Oculus.Platform.Models;
using UnityEngine;

//A standard flag for displaying information about a point of interest
//Has start and endTime for defining first mention and last seen documentation of the point of interest as well as Color attribute for 
//categorization and visual clarity
public class StandardFlag : MonoBehaviour, IFlag
{
    public int startTime { get; set; }
    public int endTime { get; set; }
    public Vector3 position { get; set; }
    public GameObject flagVisualTextComponent { get; set; }
    public GameObject flagVisualIndicator { get; set; }
    public string header { get; set; }
    public string info { get; set; }
    Color flagColor;

    public void CheckForTimeAdvance(float newTime){
        if(newTime > endTime || newTime < startTime){
            Debug.Log("New Time outside Timezone of Object: " + gameObject.name);
        }
    }

    public StandardFlag(int startTime, int endTime, Vector3 position, string header, string info, Color flagColor){
        this.startTime = startTime;
        this.endTime = endTime;
        this.position = position;
        this.header = header;
        this.info = info;
        this.flagColor = flagColor;
    }

    void Start()
    {
        GetFlagComponents();
    }

    public void DisplayInformation()
    {
        try{
            flagVisualTextComponent.SetActive(true);
            flagVisualIndicator.SetActive(false);
        } catch {
            Debug.LogError("Couldn't find a FlagVisualTextComponent in children of Object: "+ gameObject.name);
        }
        throw new System.NotImplementedException();
    }

    public void StopDisplayingInformation(){
        try{
            flagVisualTextComponent.SetActive(false);
            flagVisualIndicator.SetActive(false);
        } catch {
            Debug.LogError("Couldn't find a FlagVisualTextComponent in children of Object: "+ gameObject.name);
        }
    }

    public void GetFlagComponents()
    {
        try{
            flagVisualTextComponent = transform.FindChildRecursive("FlagVisualTextComponent").gameObject;
        } catch {
            Debug.LogError("Couldn't find a FlagVisualTextComponent in children of Object: "+ gameObject.name);
        }

        try{
            flagVisualIndicator = transform.FindChildRecursive("FlagVisualIndicator").gameObject;
        } catch {
            Debug.LogError("Couldn't find a FlagVisualIndicator in children of Object: "+ gameObject.name);
        }
    }
}
