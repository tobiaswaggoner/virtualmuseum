using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreviewTerritorySetup : MonoBehaviour
{
    public Material territoryMaterial;
    private float time;
    public List<GameObject> testingVertices1 = new List<GameObject>();
    public List<GameObject> testingVertices2 = new List<GameObject>();
    private Coroutine testingRoutine;
    public TMP_Text timeDisplayTerritory;
    public TMP_Text timeDisplayMarker;


    // Start is called before the first frame update
    void Start()
    {
        Territory.ResetStatics();
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.clear);
        tex.Reinitialize(2 * Territory.textureResolution, Territory.textureResolution);
        Territory.maskTexture2D = tex;
        Territory.maskTexture = new RenderTexture(2 * Territory.textureResolution, Territory.textureResolution, 0);
        Territory.maskTextureCreated = true;
        territoryMaterial.SetTexture("_MaskTex", Territory.maskTexture);
        //create testing Points:
        new Territory((int)Territory.currentTime, new Vector3(0,0,0), "testTerritory" + Territory.currentTime, "none", Color.blue);
    
        foreach(var vertex in testingVertices1){
            Territory.selectedTerritory.AddPointToCurrentBorder(vertex.transform.position, 704, Color.blue);
        }
        AdvanceTerritories();
        Debug.Log("TerritoryTime: " + Territory.currentTime);
        foreach(var vertex in testingVertices2){
            Territory.selectedTerritory.AddPointToCurrentBorder(vertex.transform.position, 705, Color.blue);
        }
        RegressTerritories();
        testingRoutine = StartCoroutine(TestingRoutine());
    }

    private IEnumerator TestingRoutine(){
        while(true){
            yield return new WaitForSeconds(3f);
            AdvanceTerritories();
            timeDisplayTerritory.text = Territory.currentTime.ToString();
            foreach(var g in testingVertices2){
                g.SetActive(true);
            }
            foreach(var g in testingVertices1){
                g.SetActive(false);
            }
            yield return new WaitForSeconds(3f);
            RegressTerritories();
            timeDisplayTerritory.text = Territory.currentTime.ToString();
            foreach(var g in testingVertices1){
                g.SetActive(true);
            }
            foreach(var g in testingVertices2){
                g.SetActive(false);
            }
        }
    }  

    // Update is called once per frame
    void Update()
    {   
        timeDisplayMarker.text = StandardFlag.currentTime.ToString();
        if(Territory.Territories.Count == 0) {
            return;
            }
        foreach(var territory in Territory.Territories){
            if(!territory.InterpolateBorders(time)){
                territory.DrawCurrentTerritory();
            }
        }
        time += Time.deltaTime;
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
