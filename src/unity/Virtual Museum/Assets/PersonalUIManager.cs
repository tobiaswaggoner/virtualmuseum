using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalUIManager : MonoBehaviour
{
    public GameObject TimeButtons;
    public GameObject CityList;
    // Start is called before the first frame update
    public void ActivateCityList(){
        TimeButtons.SetActive(false);
        CityList.SetActive(true);
    }

    public void ActivateTimeButtons(){
        TimeButtons.SetActive(true);
        CityList.SetActive(false);
    }
}
