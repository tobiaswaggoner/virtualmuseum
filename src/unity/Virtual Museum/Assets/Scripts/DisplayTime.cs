using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTime : MonoBehaviour
{
    public TMP_Text timedisplay;

    // Update is called once per frame
    void Update()
    {
        timedisplay.text = "Zeit: " + StandardFlag.currentTime.ToString();
    }
}
