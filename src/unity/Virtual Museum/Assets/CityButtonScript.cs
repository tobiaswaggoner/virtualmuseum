using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityButtonScript : MonoBehaviour
{
    public StandardFlag personalStandardFlag;

    public void DisplayCity(){
        personalStandardFlag.transform.GetComponentInChildren<LineRenderer>().enabled = true;
    }

    public void HideCity(){
        personalStandardFlag.transform.GetComponentInChildren<LineRenderer>().enabled = false;
    }
}
