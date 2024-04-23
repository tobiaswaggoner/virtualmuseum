using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Oculus.Interaction.Grab;
using System.Linq;
using Unity.VisualScripting;
using System.Threading.Tasks;
using UnityEditor.Build;

public class CSVInterpreter : MonoBehaviour
{
    public LayerMask maskToIgnore;
    public float edgeBuffer = 0.001f;
    public float lowestX = 0;
    public float lowestY = 0;
    public float highestX = 0;
    public float highestY = 0;

    public float desiredWidth = 6;
    public float desiredHeight = 3;
    private Vector3 topLeftCorner = new Vector3(0,0,0);
    private Vector3 bottomRightCorner = new Vector3(0,0,0); 

    public LineRenderer lineRenderer = new LineRenderer();

    public GameObject pointPrefab;

    public TextAsset inputText;
    private string[] data;
    public List<float[]> mapCoordinates = new List<float[]>();
    public List<GameObject> points = new List<GameObject>();
    public Dictionary<int , List<StandardFlag>> erscheinungsMap = new Dictionary<int, List<StandardFlag>>();

    public bool calculatedStuff = false;

    // TEST ----------------------
    int testPeriod = 704;
    // TEST ----------------------

    // Update is called once per frame
    void Update()
    {
        //TEST -----------------------------------
        if(Input.GetKeyDown(KeyCode.Return)){
            DisplayFromPeriod(testPeriod);
            testPeriod = getNextPeriod(testPeriod);
        }
        //TEST -----------------------------------
        if(!calculatedStuff) return;
        DrawMapOutline();
        DrawPointsOnMap();
    }

    public int getNextPeriod(int currentPeriod){
        int newPeriod = currentPeriod;
        newPeriod ++;
        while(!erscheinungsMap.TryGetValue(newPeriod, out var v)) {
            if(newPeriod < 2100){
                newPeriod ++;
            } else {
                newPeriod = currentPeriod;
                Debug.LogError("Tried to get period at a point past dataset.\n No period higher than " + currentPeriod + " exists");
            }
        }
        return newPeriod;
    }

    private int getLastPeriod(){
        int newPeriod = currentPeriod;
        newPeriod --;
        while(!erscheinungsMap.TryGetValue(newPeriod, out var v)) {
            if(newPeriod < 500){
                newPeriod --;
            } else {
                newPeriod = currentPeriod;
                Debug.LogError("Tried to get period at a point past dataset.\n No period higher than " + currentPeriod + " exists");
            }
        }
        return newPeriod;
    }

    private void DrawMapOutline(){
        lineRenderer.SetPosition(0, topLeftCorner);
        lineRenderer.SetPosition(1, new Vector3(bottomRightCorner.x, bottomRightCorner.y, topLeftCorner.z));
        lineRenderer.SetPosition(2, bottomRightCorner);
        lineRenderer.SetPosition(3, new Vector3(topLeftCorner.x, bottomRightCorner.y, bottomRightCorner.z));
    }

    private void DrawPointsOnMap(){
        for(int i = 0; i < mapCoordinates.Count; i ++){
            float newX = topLeftCorner.x + mapCoordinates[i][0] * desiredWidth;
            float newZ = topLeftCorner.z - mapCoordinates[i][1] * desiredHeight;
            //Raycast Down to get Y
            Vector3 oldPos = new Vector3(newX, topLeftCorner.y, newZ);
            Ray ray = new Ray(oldPos, Vector3.down);
            Physics.Raycast(ray, out RaycastHit hit, 5f, ~maskToIgnore);

            Vector3 newPosition = hit.point;
            points[i].transform.position = newPosition;
        }
    }

    public void UpdateDesiredCorners(Vector3 newTopLeft, Vector3 newBottomRight){
        topLeftCorner = newTopLeft;
        bottomRightCorner = newBottomRight;

        desiredWidth = bottomRightCorner.x - topLeftCorner.x;
        desiredHeight = topLeftCorner.z - bottomRightCorner.z;
    }

    public async Task CalculateStuff(){
        List<int> ersterwähnungen = new List<int>();
        List<String> ort = new List<string>();
        List<String> landkreis = new List<string>();
        List<String> gPSOld = new List<string>();
        List<float[]> gPS = new List<float[]>();

        data = inputText.text.Split(new String[] {";" , "\n"}, System.StringSplitOptions.None);
        int j = 0;
        for(int i = 0; i < data.Length; i ++){
            if(data[i] == "") continue;
            if(j == 0){
                ersterwähnungen.Add(int.Parse(data[i]));
            } else if(j == 1){
                ort.Add(data[i]);
            } else if(j == 2){
                landkreis.Add(data[i]);
            } else if(j == 3){
                gPSOld.Add(data[i]);
                j = 0;
                continue;
            }
            j ++;
        }

        for(int i = 0; i < gPS.Count; i ++){
            gPS[i] = new float[2];
        }
        List<string> temp = new List<string>();
        foreach(string s in gPSOld){
            if(s != "GPS" && s != "-1"){
                var t = s.Split(new string[] {","}, StringSplitOptions.None);
                var t1 = new float[2];
        

                t[0] = t[0].Replace(".", ",");
                t[1] = t[1].Replace(".", ",");
                t1[0] = float.Parse(t[0]);
                t1[1] = float.Parse(t[1]);
                
                //Debug.Log(t[0] + " ; " + t1[0]);
                //Debug.Log(t[1] + " ; " + t1[1]);

                gPS.Add(t1);
                points.Add(Instantiate(pointPrefab));
            } 
        }

        //calculate highest and lowest points for gps x and y coordinates
        lowestX = gPS.Min(xy => xy[0]); //top left corner of Map X
        lowestY = gPS.Min(xy => xy[1]); //top left corner of Map Y
        highestX = gPS.Max(xy => xy[0]);//bottom right corner of Map X
        highestY = gPS.Max(xy => xy[1]);//bottom right corner of Map Y

        Debug.Log("lowestX: " + lowestX + ", lowestY: " + lowestY);
        Debug.Log("highestX: " + highestX + ", highestY: " + highestY);

        for (int i = 0; i < gPS.Count; i ++){
            float mapX = (gPS[i][0] - lowestX) / (highestX - lowestX);
            float mapY = (gPS[i][1] - lowestY) / (highestY - lowestY);
            mapCoordinates.Add(new float[2]{mapX, mapY});
        }

        for(int i = 0; i < mapCoordinates.Count; i ++){
            float newX = topLeftCorner.x + mapCoordinates[i][0] * desiredWidth;
            float newZ = topLeftCorner.z - mapCoordinates[i][1] * desiredHeight;
            //Raycast Down to get Y
            Vector3 oldPos = new Vector3(newX, topLeftCorner.y, newZ);
            Ray ray = new Ray(oldPos, Vector3.down);
            Physics.Raycast(ray, out RaycastHit hit, 5f, ~maskToIgnore);

            Vector3 newPosition = hit.point;
            StandardFlag newFlag = new StandardFlag(ersterwähnungen[i], hit.point, points[i].transform, Color.red, ort[i]
                                                    , "Landkreis: " + landkreis[i] + "\n GPS: " + gPSOld[i]);
            if(!erscheinungsMap.TryGetValue(ersterwähnungen[i], out var v)){
                erscheinungsMap[ersterwähnungen[i]] = new List<StandardFlag>();
            }
            newFlag.transform.GetComponent<MeshRenderer>().enabled = false;
            erscheinungsMap[ersterwähnungen[i]].Add(newFlag);
            points[i].transform.position = newPosition;
        }

        calculatedStuff = true;
        lineRenderer.enabled = true;
    }
    
    public int currentPeriod = 704;
    public void DisplayFromPeriod(int period = 704, bool up = true){
        if(!erscheinungsMap.TryGetValue(period, out var v)) return;
        for(int i = 0; i < erscheinungsMap[currentPeriod].Count; i ++){
            StandardFlag currentFlag = erscheinungsMap[currentPeriod][i];
            if(up){
                currentFlag.SetColor(Color.yellow);    
            } else {
                currentFlag.Deactivate();
            }
        }

        StartCoroutine(DisplayMarkersWithDelay(erscheinungsMap[period], 0.1f));
        currentPeriod = period;
    }

    private IEnumerator DisplayMarkersWithDelay(List<StandardFlag> markers, float delay){
        for(int i = 0; i < markers.Count; i ++){
            StandardFlag currentFlag = markers[i];
            currentFlag.Activate();
            yield return new WaitForSeconds(delay);
        }
        yield return null;
    }
}
