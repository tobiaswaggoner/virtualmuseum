using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TerritoryController : MonoBehaviour
{
    public Material territoryMaterial;
    float time;

    public bool testing = true;

    public List<GameObject> testingVertices1 = new List<GameObject>();
    public List<GameObject> testingVertices2 = new List<GameObject>();

    private Coroutine testingRoutine;
    
    void Start()
    {
        Territory.ResetStatics();
        Territory.maskTexture2D = new Texture2D(2 * Territory.textureResolution, Territory.textureResolution, TextureFormat.RGBA32, false);
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
            foreach(var g in testingVertices2){
                g.SetActive(true);
            }
            foreach(var g in testingVertices1){
                g.SetActive(false);
            }
            yield return new WaitForSeconds(3f);
            RegressTerritories();
            foreach(var g in testingVertices1){
                g.SetActive(true);
            }
            foreach(var g in testingVertices2){
                g.SetActive(false);
            }
        }
    }   

    void Update(){

        if(Territory.Territories.Count == 0) {return;}
        foreach(var territory in Territory.Territories){
            if(!territory.InterpolateBorders(time)){
                territory.DrawCurrentTerritory();
            }
        }
        time += Time.deltaTime;

        if(!testing) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 point = hit.point;
                if(Territory.selectedTerritory == null){
                    new Territory((int)Territory.currentTime, point, "testTerritory" + Territory.currentTime, "none", Color.red);
                    return;
                }
                Territory.selectedTerritory.AddPointToCurrentBorder(point, (int) Territory.currentTime, Color.blue);
            }
        } else if (Input.GetMouseButtonDown(1)){
            AdvanceTerritories();
            Debug.Log("TerritoryTime: " + Territory.currentTime);
        } else if(Input.GetKeyDown(KeyCode.Space)){
            RegressTerritories();
            Debug.Log("TerritoryTime: " + Territory.currentTime);
        }
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
