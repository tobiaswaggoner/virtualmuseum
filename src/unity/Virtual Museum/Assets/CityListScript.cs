using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityListScript : MonoBehaviour
{
    public GameObject uiCityPrefab;
    List<CityButtonScript> cityButtonScripts = new List<CityButtonScript>();
    public void CreateCityUIRepresentation(StandardFlag backlinkFlag){
        CityButtonScript cityButtonScript = Instantiate(uiCityPrefab, transform).GetComponent<CityButtonScript>();
        cityButtonScripts.Add(cityButtonScript);
        cityButtonScript.personalStandardFlag = backlinkFlag;
        cityButtonScript.transform.GetComponentInChildren<TMP_Text>().text = backlinkFlag.header;

        cityButtonScripts[0].DisplayCity();
    }

    public void ClearCities(){
        foreach(CityButtonScript cityButtonScript in cityButtonScripts){
            Destroy(cityButtonScript.gameObject);
        }
        cityButtonScripts.Clear();
    }
}
