using System.Collections;
using System.Collections.Generic;
using OVR.OpenVR;
using UnityEditor;
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
    static public int textureResolution = 512;
    static public float currentTime = 704;
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

    public Territory(int startTime, Vector3 position, string header, string info, Color color){
        this.startTime = startTime;
        this.position = position;
        this.header = header;
        this.info = info;
        Color = color;
        if(maskTextureCreated == false) {
            maskTexture2D = new Texture2D(2 * textureResolution, textureResolution, TextureFormat.RGBA32, false);
            maskTexture = new RenderTexture(2 * textureResolution, textureResolution, 0);
            maskTextureCreated = true;
        }
        selectedTerritory = this;
        Debug.Log("Territory created!");
        Territories.Add(this);
    }

    static void UpdateTerritories(float t){
        currentTime = t;
        foreach(var territory in Territories){
            territory.InterpolateTerritories(currentTime);
        }
        
    }

    void InterpolateTerritories(float time){
        TerritoryBorder previous = null;
        TerritoryBorder next = null;
        foreach(var border in Borders){
            if(border.startTime <= time && (previous == null || border.startTime > previous.startTime)){
                previous = border;
            }
            if(border.startTime >= time && (next == null || border.startTime < next.startTime)){
                next = border;
            }
        }
        

        if(previous != null && next != null && previous != next){
            float t = Mathf.InverseLerp(previous.startTime, next.startTime, time);
            GenerateMaskTexture(previous, next, t);
        }
    }

    static void GenerateMaskTexture(TerritoryBorder previous, TerritoryBorder next, float t){
        Color clearColor = new Color(0,0,0,0);
        for(int y = 0; y < textureResolution; y ++){
            for(int x = 0; x < 2*textureResolution; x ++){
                maskTexture2D.SetPixel(x, y, clearColor);
            }
        }

        DrawTerritory(previous, next, t);
        maskTexture2D.Apply();
        Graphics.Blit(maskTexture2D, maskTexture);
    }

    static void DrawTerritory(TerritoryBorder previous, TerritoryBorder next, float t){
        Color interpolatedColor = Color.Lerp(previous.Color, next.Color, t);

        for(int i = 0; i < previous.Points.Count; i ++){
            Vector3 previousPoint = previous.Points[i];
            Vector3 nextPoint = next.Points[i];
            Vector3 interpolatedPoint = Vector3.Lerp(previousPoint, nextPoint, t);

            int x = Mathf.RoundToInt(interpolatedPoint.x * textureResolution * 2);
            int y = Mathf.RoundToInt(interpolatedPoint.z * textureResolution);

            if(x >= 0 && x < textureResolution * 2 && y >= 0 && y < textureResolution){
                maskTexture2D.SetPixel(x, y, interpolatedColor);
            }
        }
    }

    public void AddPointToCurrentBorder(Vector3 point, int t, Color c){
        TerritoryBorder currentBorder = Borders.Find(b => b.startTime == currentTime);
        if(currentBorder == null){
            currentBorder = new TerritoryBorder(t, c);
            Borders.Add(currentBorder);
            Debug.Log("Created Border! " + t);
        }
        currentBorder.Points.Add(point);
        Debug.Log("Point created! " + currentBorder.startTime);
    }


}
