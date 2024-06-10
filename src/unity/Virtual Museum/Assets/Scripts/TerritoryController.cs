using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoryController : MonoBehaviour
{
    bool working = false;
    float time = 0;
    public Material territoryMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!working) return;  
        Territory.ResetStatics();
        Territory.maskTexture2D = new Texture2D(2 * Territory.textureResolution, Territory.textureResolution, TextureFormat.RGBA32, false);
        Territory.maskTexture = new RenderTexture(2 * Territory.textureResolution, Territory.textureResolution, 0);
        Territory.maskTextureCreated = true;
        territoryMaterial.SetTexture("_MaskTex", Territory.maskTexture); 
    }

    void SetUp(){
        Territory.ResetStatics();
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.clear);
        tex.Reinitialize(2 * Territory.textureResolution, Territory.textureResolution);
        Territory.maskTexture2D = tex;
        Territory.maskTexture = new RenderTexture(2 * Territory.textureResolution, Territory.textureResolution, 0);
        Territory.maskTextureCreated = true;
        territoryMaterial.SetTexture("_MaskTex", Territory.maskTexture);
        //sets Territory.selectedTerritory to itself -> added points via AddPointToCurrentBorder() will be added to this territory at this time
    }

    void CreateTerritory(Vector3 point, string name, Color color){
        new Territory((int)Territory.currentTime, point, name + " " + Territory.currentTime, "none", color);
    }
    
    public void AdvanceTerritories(){
        if(Territory.Territories.Count == 0) return;
        time = 0;
        foreach(Territory territory in Territory.Territories){
            territory.previousBorder = territory.currentBorder;
        }
        Territory.currentTime ++;
        foreach(Territory territory in Territory.Territories){
            territory.InitializeIntermediatePoints();
        }
    }

    public void RegressTerritories(){
        if(Territory.Territories.Count == 0) return;
        time = 0;
        foreach(Territory territory in Territory.Territories){
            territory.previousBorder = territory.currentBorder;
        }
        Territory.currentTime --;
        foreach(Territory territory in Territory.Territories){
            territory.InitializeIntermediatePoints();
        }
    }
}
