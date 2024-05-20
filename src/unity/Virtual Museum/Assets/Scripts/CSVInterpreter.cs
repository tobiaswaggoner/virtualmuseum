using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Threading.Tasks;

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
    private Vector3 topLeftCorner = Vector3.zero;
    private Vector3 bottomRightCorner = Vector3.zero;

    public LineRenderer lineRenderer;

    public GameObject pointPrefab;

    public TextAsset inputText;
    private string[] data;
    public List<float[]> mapCoordinates = new List<float[]>();
    public List<GameObject> points = new List<GameObject>();
    public Dictionary<int, List<StandardFlag>> erscheinungsMap = new Dictionary<int, List<StandardFlag>>();

    private bool calculatedStuff = false;

    // Update is called once per frame
    void Update()
    {
        if (!calculatedStuff) return;

        // TEST -----------------------------------
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StandardFlag.NextPeriod();
        }
        // TEST -----------------------------------
    }

    private void DrawMapOutline()
    {
        lineRenderer.SetPosition(0, topLeftCorner);
        lineRenderer.SetPosition(1, new Vector3(bottomRightCorner.x, bottomRightCorner.y, topLeftCorner.z));
        lineRenderer.SetPosition(2, bottomRightCorner);
        lineRenderer.SetPosition(3, new Vector3(topLeftCorner.x, bottomRightCorner.y, bottomRightCorner.z));
    }

    public void UpdateDesiredCorners(Vector3 newTopLeft, Vector3 newBottomRight)
    {
        topLeftCorner = newTopLeft;
        bottomRightCorner = newBottomRight;

        desiredWidth = bottomRightCorner.x - topLeftCorner.x;
        desiredHeight = topLeftCorner.z - bottomRightCorner.z;
    }

    public void CalculateStuff()
    {
        // Parsing CSV data
        var records = ParseCSVData(inputText.text);

        // Processing GPS coordinates
        var gpsCoordinates = ProcessGPSCoordinates(records.gpsOld);

        // Calculate map coordinates
        CalculateMapCoordinates(gpsCoordinates);

        // Place markers on the map
        PlaceMarkers(records, gpsCoordinates);

        calculatedStuff = true;
        lineRenderer.enabled = true;
    }

    private (List<int> ersterwähnungen, List<string> ort, List<string> landkreis, List<string> gpsOld) ParseCSVData(string csvText)
    {
        var ersterwähnungen = new List<int>();
        var ort = new List<string>();
        var landkreis = new List<string>();
        var gpsOld = new List<string>();

        data = csvText.Split(new string[] { ";", "\n" }, StringSplitOptions.None);
        for (int i = 0, j = 0; i < data.Length; i++)
        {
            if (string.IsNullOrEmpty(data[i])) continue;

            switch (j)
            {
                case 0:
                    if (int.TryParse(data[i], out int year)) 
                        ersterwähnungen.Add(year);
                    break;
                case 1:
                    ort.Add(data[i]);
                    break;
                case 2:
                    landkreis.Add(data[i]);
                    break;
                case 3:
                    gpsOld.Add(data[i]);
                    j = -1; // Reset to start of next record
                    break;
            }
            j++;
        }

        return (ersterwähnungen, ort, landkreis, gpsOld);
    }

    private List<float[]> ProcessGPSCoordinates(List<string> gpsOld)
    {
        var gpsCoordinates = new List<float[]>();

        foreach (string s in gpsOld)
        {
            if (s != "GPS" && s != "-1")
            {
                var t = s.Split(',');
                if (t.Length == 2 && float.TryParse(t[0], out float lat) && float.TryParse(t[1], out float lon))
                {
                    gpsCoordinates.Add(new float[] { lat, lon });
                    points.Add(Instantiate(pointPrefab));
                }
            }
        }

        return gpsCoordinates;
    }

    private void CalculateMapCoordinates(List<float[]> gpsCoordinates)
    {
        if (gpsCoordinates.Count == 0) return;

        lowestX = gpsCoordinates.Min(xy => xy[0]);
        lowestY = gpsCoordinates.Min(xy => xy[1]);
        highestX = gpsCoordinates.Max(xy => xy[0]);
        highestY = gpsCoordinates.Max(xy => xy[1]);

        Debug.Log($"lowestX: {lowestX}, lowestY: {lowestY}");
        Debug.Log($"highestX: {highestX}, highestY: {highestY}");

        foreach (var gps in gpsCoordinates)
        {
            float mapX = (gps[0] - lowestX) / (highestX - lowestX);
            float mapY = (gps[1] - lowestY) / (highestY - lowestY);
            mapCoordinates.Add(new float[] { mapX, mapY });
        }
    }

    private void PlaceMarkers((List<int> ersterwähnungen, List<string> ort, List<string> landkreis, List<string> gpsOld) records, List<float[]> gpsCoordinates)
    {
        for (int i = 0; i < mapCoordinates.Count; i++)
        {
            float newX = topLeftCorner.x + mapCoordinates[i][0] * desiredWidth;
            float newZ = topLeftCorner.z - mapCoordinates[i][1] * desiredHeight;
            Vector3 oldPos = new Vector3(newX, topLeftCorner.y, newZ);

            if (Physics.Raycast(new Ray(oldPos, Vector3.down), out RaycastHit hit, 5f, ~maskToIgnore))
            {
                Vector3 newPosition = hit.point + new Vector3(0, 0.1f, 0);
                var newFlag = new StandardFlag(records.ersterwähnungen[i], hit.point, points[i].transform, Color.red, records.ort[i], $"Landkreis: {records.landkreis[i]}\n GPS: {records.gpsOld[i]}");

                if (!erscheinungsMap.TryGetValue(records.ersterwähnungen[i], out var flagList))
                {
                    erscheinungsMap[records.ersterwähnungen[i]] = new List<StandardFlag>();
                }
                erscheinungsMap[records.ersterwähnungen[i]].Add(newFlag);
                points[i].transform.position = newPosition;
            }
            else
            {
                Debug.LogWarning($"Raycast did not hit for position newX: {newX}, newZ: {newZ}");
            }
        }
    }


    public int currentPeriod = 704;
    public void DisplayFromPeriod(int period = 704, bool up = true)
    {
        if (!erscheinungsMap.TryGetValue(period, out var newFlags)) return;

        foreach (var currentFlag in erscheinungsMap[currentPeriod])
        {
            if (up)
            {
                currentFlag.SetColor(Color.yellow);
            }
            else
            {
                currentFlag.Deactivate();
            }
        }

        StartCoroutine(DisplayMarkersWithDelay(newFlags, 0.1f));
        currentPeriod = period;
    }

    private IEnumerator DisplayMarkersWithDelay(List<StandardFlag> markers, float delay)
    {
        foreach (var marker in markers)
        {
            marker.Activate();
            yield return new WaitForSeconds(delay);
        }
    }
}
