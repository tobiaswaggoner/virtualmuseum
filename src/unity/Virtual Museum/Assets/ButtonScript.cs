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
}
