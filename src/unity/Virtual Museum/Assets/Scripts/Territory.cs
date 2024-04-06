using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///A flag for displaying the development of a territory across a time span
///A territory is made up of Borders. Each border defining the current borders during a specific point within the overall time span
///Includes start and end time, as well as definable position for information display to be able to represent a capital or center of the
///territory. Also includes Color for categorization and visual clarity
public class Territory : MonoBehaviour, IFlag
{
    public int startTime { get; set; }
    public int endTime { get; set; }
    public Vector3 position { get; set; }
    public GameObject flagVisualTextComponent { get; set; }
    public GameObject flagVisualIndicator { get; set; }
    public string header { get; set; }
    public string info { get; set; }

    Color territoryColor;
    Border currentBorder;

    int currentStartingTime;
    int currentEndingTime;
    int amountOfEdges;
    List<Vector3> oldEdges = new List<Vector3>();
    private Coroutine animationCoroutine;

    public Territory(int startTime, int endTime, Vector3 position, string header, string info){
        this.startTime = startTime;
        this.endTime = endTime;
        this.position = position;
        this.header = header;
        this.info = info;
    }

    void Start()
    {
        GetFlagComponents();
    }

    public void CheckForTimeAdvance(float newTime){
        if(newTime > endTime || newTime < startTime){
            Debug.Log("New Time outside Timezone of Object: " + gameObject.name);
            return;
        }

        if(newTime > currentEndingTime){
            ///advance time forward to next Border, also recursively call CheckForTimeAdvance
            currentBorder = currentBorder.nextBorder;
            DrawNewBorders();
            currentStartingTime = currentBorder.startingTime;
            currentEndingTime = currentBorder.endingTime;
            animationCoroutine = StartCoroutine(AnimateBorders());
            CheckForTimeAdvance(newTime);
            return;
        }

        if(newTime < currentStartingTime){
            ///advance time backward to last Border, also recursively call CheckForTimeAdvance
            currentBorder = currentBorder.lastBorder;
            DrawNewBorders();
            currentStartingTime = currentBorder.startingTime;
            currentEndingTime = currentBorder.endingTime;
            CheckForTimeAdvance(newTime);
            return;
        }

        Debug.LogError("Time outside of Territory history");
    }


    ///Display the Information of current Flag
    public void DisplayInformation()
    {
        try{
            flagVisualTextComponent.SetActive(true);
        } catch {
            Debug.LogError("Couldn't find a FlagVisualTextComponent attached to Object: "+ gameObject.name);
        }
        throw new System.NotImplementedException();
    }

    public void StopDisplayingInformation(){
        try{
            flagVisualTextComponent.SetActive(false);
        } catch {
            Debug.LogError("Couldn't find a FlagVisualTextComponent attached to Object: "+ gameObject.name);
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

    public void InitialiseNewBorders(List<Border> borders){
        Border startBorder = borders[0];
        Border endBorder = borders[borders.Count - 1];
        for(int i = 0; i < borders.Count; i ++){
            if(i < borders.Count - 1){
                borders[i].nextBorder = borders[i + 1];
            }
            if(i > 0){
                borders[i].lastBorder = borders[i  -1];
            }
            borders[i].endBorder = endBorder;
            borders[i].startBorder = startBorder;
        }
    }

    public void AddNewBorder(Border newBorder){
        currentBorder.endBorder.nextBorder = newBorder;
        newBorder.lastBorder = currentBorder.endBorder;

        Border originalBorder = currentBorder;
        currentBorder = currentBorder.startBorder;
        while(!currentBorder.nextBorder.Equals(null)){
            currentBorder.endBorder = newBorder;
        }

        currentBorder = originalBorder;
    }

    private void DrawNewBorders(){
        ///Get new edges from current Border and adjust texture
        throw new System.NotImplementedException();
    }

    public IEnumerator AnimateBorders(){
        List<Vector3> newEdges = currentBorder.edges;
        ///interpolate between each point in oldEdges and newEdges to create a Territory animation
        throw new System.NotImplementedException();
    }

    public void SetBorder(Border border){currentBorder = border;}
}
