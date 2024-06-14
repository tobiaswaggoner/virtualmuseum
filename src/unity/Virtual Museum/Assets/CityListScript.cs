using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityListScript : MonoBehaviour
{
    public GameObject uiCityPrefab;
    List<CityButtonScript> cityButtonScripts = new List<CityButtonScript>();
    public void CreateCityUIRepresentation(StandardFlag backlinkFlag){
        CityButtonScript cityButtonScript = Instantiate(uiCityPrefab, transform).GetComponent<CityButtonScript>();
        cityButtonScripts.Add(cityButtonScript);
        cityButtonScript.personalStandardFlag = backlinkFlag;
    }

    public void ClearCities(){
        foreach(CityButtonScript cityButtonScript in cityButtonScripts){
            Destroy(cityButtonScript.gameObject);
        }
        cityButtonScripts.Clear();
    }
}
