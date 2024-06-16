using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalUIManager : MonoBehaviour
{
    public GameObject TimeButtons;
    public GameObject CityList;
    public GameObject EnterButton;
    public GameObject ExitButton;
    // Start is called before the first frame update
    public void ActivateCityList(){
        TimeButtons.SetActive(false);
        CityList.SetActive(true);
    }

    public void ActivateTimeButtons(){
        TimeButtons.SetActive(true);
        CityList.SetActive(false);
        foreach(Transform t in CityList.transform){
            Destroy(t.gameObject);
        }
    }

    public void ActivateCubeMap(){
        if(StandardFlag.selectedFlag != null){
            StandardFlag.selectedFlag.ShowCubeMap();
        }
        EnterButton.SetActive(false);
        ExitButton.SetActive(true);
    }
    public void DeactivateCubeMap(){
        if(StandardFlag.selectedFlag != null){
            StandardFlag.selectedFlag.HideCubeMap();
        }
        EnterButton.SetActive(true);
        ExitButton.SetActive(false);
    }
}
