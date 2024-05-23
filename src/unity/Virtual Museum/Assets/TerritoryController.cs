using UnityEngine;

public class TerritoryController : MonoBehaviour
{
    public Material territoryMaterial;
    float time;
    
    void Start()
    {
        Territory.ResetStatics();
        Territory.maskTexture2D = new Texture2D(2 * Territory.textureResolution, Territory.textureResolution, TextureFormat.RGBA32, false);
        Territory.maskTexture = new RenderTexture(2 * Territory.textureResolution, Territory.textureResolution, 0);
        Territory.maskTextureCreated = true;
        Debug.Log("createdTexture");
        territoryMaterial.SetTexture("_MaskTex", Territory.maskTexture);
        Debug.Log(territoryMaterial.GetTexture("_MaskTex"));
    }

    void Update(){
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

        if(Territory.Territories.Count == 0) return;
        foreach(var territory in Territory.Territories){
            if(!territory.InterpolateBorders(time)){
                territory.DrawCurrentTerritory();
            }
        }
        time += Time.deltaTime;
    }

    private void AdvanceTerritories(){
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

    private void RegressTerritories(){
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
