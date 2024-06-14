using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CityButtonScript : MonoBehaviour
{
    public StandardFlag personalStandardFlag;
    private bool isCityShowing = false;

    public void DisplayCity(){
        if(isCityShowing) {
            isCityShowing = false;
            personalStandardFlag.HideLineRenderer();
            personalStandardFlag.HideText();
        } else {
            isCityShowing = true;
            personalStandardFlag.ShowText();
            personalStandardFlag.ShowLineRenderer();
        }
    }

    public void HideCity(){
        personalStandardFlag.HideLineRenderer();
        personalStandardFlag.HideText();
    }
}
