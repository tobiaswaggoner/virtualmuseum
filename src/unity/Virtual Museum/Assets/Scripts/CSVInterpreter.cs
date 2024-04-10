using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Oculus.Interaction.Grab;
using System.Linq;
using Unity.VisualScripting;

public class CSVInterpreter : MonoBehaviour
{

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
    public List<String> ersterwähnungen = new List<string>();
    public List<String> ort = new List<string>();
    public List<String> landkreis = new List<string>();
    public List<String> gPSOld = new List<string>();
    public List<float[]> gPS = new List<float[]>();
    public List<float[]> mapCoordinates = new List<float[]>();
    public List<GameObject> points = new List<GameObject>();

    public bool calculatedStuff = false;

    // Update is called once per frame
    void Update()
    {
        if(!calculatedStuff) return;
        DrawMapOutline();
        DrawPointsOnMap();
    }

    private void DrawMapOutline(){
        float offsetZ = 3;
        topLeftCorner = new Vector3(- desiredWidth / 2, desiredHeight / 2, offsetZ);
        bottomRightCorner = new Vector3(desiredWidth / 2, - desiredHeight / 2, offsetZ);

        lineRenderer.SetPosition(0, topLeftCorner);
        lineRenderer.SetPosition(1, new Vector3(bottomRightCorner.x, topLeftCorner.y, offsetZ));
        lineRenderer.SetPosition(2, bottomRightCorner);
        lineRenderer.SetPosition(3, new Vector3(topLeftCorner.x, bottomRightCorner.y, offsetZ));

    }

    private void DrawPointsOnMap(){
        for(int i = 0; i < mapCoordinates.Count - 1; i ++){
            
            float newX = topLeftCorner.x + mapCoordinates[i][0] * desiredWidth;
            float newY = topLeftCorner.y - mapCoordinates[i][1] * desiredHeight;

            Vector3 newPosition = new Vector3(newX, newY, 2.9f);
            points[i].transform.position = newPosition;
        }
    }

    public void CalculateStuff(){
        data = inputText.text.Split(new String[] {";" , "\n"}, System.StringSplitOptions.None);
        int j = 0;
        for(int i = 0; i < data.Length; i ++){
            if(data[i] == "") continue;
            if(j == 0){
                ersterwähnungen.Add(data[i]);
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

        for (int i = 0; i < gPS.Count - 1; i ++){
            float mapX = (gPS[i][0] - lowestX) / (highestX - lowestX);
            float mapY = (gPS[i][1] - lowestY) / (highestY - lowestY);
            mapCoordinates.Add(new float[2]{mapX, mapY});
        }

        calculatedStuff = true;
    }
}
