using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

///A flag for displaying the development of a territory across a time span
///A territory is made up of Borders. Each border defining the current borders during a specific point within the overall time span
///Includes start and end time, as well as definable position for information display to be able to represent a capital or center of the
///territory. Also includes Color for categorization and visual clarity
public class Territory : IFlag
{
    static public RenderTexture maskTexture;
    static public Texture2D maskTexture2D;
    static public List<Territory> Territories = new List<Territory>();
    static public int textureResolution = 255;
    static public float currentTime = 704;
    private float lastCurrentTime = 0;
    static public bool maskTextureCreated = false;
    static public Territory selectedTerritory;

    public int startTime { get; set; }
    public Color Color { get; set; }
    public Transform transform { get; set; }
    public Vector3 position { get; set; } //center position of the whole territory
    public List<TerritoryBorder> Borders = new List<TerritoryBorder>();
    public GameObject flagVisualTextComponent { get; set; }
    public GameObject flagVisualIndicator { get; set; }
    public string header { get; set; }
    public string info { get; set; }
    public TerritoryBorder currentBorder = null;
    public TerritoryBorder previousBorder = null;
    public TerritoryBorder interMediateBorder = null;

    public Territory(int startTime, Vector3 position, string header, string info, Color color){
        this.startTime = startTime;
        this.position = position;
        this.header = header;
        this.info = info;
        Color = color;
        selectedTerritory = this;
        Territories.Add(this);
    }

    static public void ResetStatics(){
        maskTexture = null;
        maskTexture2D = null;
        Territories = new List<Territory>();
        textureResolution = 255;
        currentTime = 704;
        maskTextureCreated = false;
        selectedTerritory = null;

    }

    public void DrawCurrentTerritory(){
        if(lastCurrentTime != currentTime){
            currentBorder = Borders.Find(b => b.startTime == currentTime);
            if(currentBorder == null || currentBorder.Points.Count == 0) {
                ClearMaskTexture();
                return;
            }
        }
        GenerateMaskTexture(currentBorder);
        lastCurrentTime = currentTime;
    }

    public bool InterpolateBorders(float t){
        currentBorder = Borders.Find(b => b.startTime == currentTime);
        if(interMediateBorder == null 
        || currentBorder == null
        || previousBorder == null
        || interMediateBorder.Points.Count == 0 
        || interMediateBorder.Points.Count > currentBorder.Points.Count) {
            ClearMaskTexture();
            return false;
        }
        for(int i = 0; i < interMediateBorder.Points.Count; i ++){
            interMediateBorder.Points[i] = Vector3.Lerp(previousBorder.Points[i], currentBorder.Points[i], t / 2);
        }

        GenerateMaskTexture(interMediateBorder);
        return true;
    }

    public void InitializeIntermediatePoints(){
        TerritoryBorder currentBorder = Borders.Find(b => b.startTime == currentTime);
        if(currentBorder == null || previousBorder == null) return;
        if(interMediateBorder == null) interMediateBorder = new TerritoryBorder(-1, previousBorder.Color);
        interMediateBorder.Points.Clear();
        foreach(var point in previousBorder.Points){
            interMediateBorder.Points.Add(point);
        }
    }

    static void ClearMaskTexture(){
        Color clearColor = new Color(0,0,0,0);
        for(int y = 0; y < textureResolution; y ++){
            for(int x = 0; x < 2*textureResolution; x ++){
                maskTexture2D.SetPixel(x, y, clearColor);
            }
        }
        maskTexture2D.Apply();
    }


    static void GenerateMaskTexture(TerritoryBorder thisBorder){
        ClearMaskTexture();
        DrawTerritory(thisBorder);
        maskTexture2D.Apply();
        Graphics.Blit(maskTexture2D, maskTexture);
    }

    private static Vector3 NormalizeToTextureSpace(Vector3 point)
    {
        //transform positions to local table transforms and positions -> subtract table transfrom position and rotate according to table rotation
        //add inverse of tablePosition
        float newPointX = point.x;
        float newPointZ = point.z + 2;
        //rotate inverse of tableRotation around y axis
        Vector3 newPoint = new Vector3(newPointX, point.y, newPointZ);
        newPoint = Quaternion.AngleAxis(180, Vector3.up) * newPoint;
        //normalize point in local position assuming y is up and down, z is forward and back
        float normalizedX = Mathf.InverseLerp(-2f, 2f, - newPoint.x);
        float normalizedZ = Mathf.InverseLerp(-1f, 1f, - newPoint.z);

        Vector3 resultingPoint = new Vector3(normalizedX, point.y, normalizedZ);

        return resultingPoint; // y is height, which we don't use for texture
    }

    static void DrawTerritory(TerritoryBorder thisBorder){
        
        List<Vector3> normalizedPoints = new List<Vector3>();

        for(int i = 0; i < thisBorder.Points.Count; i ++){

            Vector3 normalizedPoint = NormalizeToTextureSpace(thisBorder.Points[i]);
            normalizedPoints.Add(normalizedPoint);
        }

        FillPolygon(normalizedPoints, thisBorder.Color);
    }

    private static void FillPolygon(List<Vector3> points, Color color)
    {
        if (points.Count < 3)
            return; // Not a valid polygon

        // Convert points to integer coordinates
        List<Vector2Int> intPoints = new List<Vector2Int>();
        foreach (var point in points)
        {
            int x = Mathf.RoundToInt(point.x * (textureResolution * 2 - 1));
            int y = Mathf.RoundToInt(point.z * (textureResolution - 1));
            intPoints.Add(new Vector2Int(x, y));
        }

        // Find bounds of the polygon
        int minY = intPoints[0].y, maxY = intPoints[0].y;
        foreach (var point in intPoints)
        {
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }
       
        // Scanline fill algorithm
        for (int y = minY; y <= maxY; y++)
        {
            List<int> nodes = new List<int>();
            int j = intPoints.Count - 1;
            for (int i = 0; i < intPoints.Count; i++)
            {
                if (intPoints[i].y < y && intPoints[j].y >= y || intPoints[j].y < y && intPoints[i].y >= y)
                {
                    nodes.Add(intPoints[i].x + (y - intPoints[i].y) * (intPoints[j].x - intPoints[i].x) / (intPoints[j].y - intPoints[i].y));
                }
                j = i;
            }

            nodes.Sort();
            for (int i = 0; i < nodes.Count; i += 2)
            {
                if (nodes[i] >= textureResolution * 2) break;
                if (nodes[i + 1] > 0)
                {
                    if (nodes[i] < 0) nodes[i] = 0;
                    if (nodes[i + 1] > textureResolution * 2) nodes[i + 1] = textureResolution * 2;
                    for (int x = nodes[i]; x < nodes[i + 1]; x++)
                    {
                        maskTexture2D.SetPixel(x, y, color);
                    }
                }
            }
        }
    }

    public void AddPointToCurrentBorder(Vector3 point, int t, Color c){
        TerritoryBorder currentBorder = Borders.Find(b => b.startTime == currentTime);
        if(currentBorder == null){
            currentBorder = new TerritoryBorder(t, c);
            Borders.Add(currentBorder);
            this.currentBorder = currentBorder;
            Debug.Log("Created Border! " + t + " : " +this.currentBorder);
        }
        currentBorder.Points.Add(point);
    }
}
