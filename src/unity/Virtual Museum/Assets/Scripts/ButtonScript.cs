using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public void AdvanceTime(){
        StandardFlag.NextPeriod();
    }

    public void DecreaseTime(){
        StandardFlag.LastPeriod();
    }

    public void DisplayBlock(){
        StandardFlag.DisplayBlock(int.Parse(gameObject.transform.name));
    }

    public void DisplayInformation(){
        
    }
}
