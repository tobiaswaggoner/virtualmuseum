using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static OVRHand;

public class GeneralHandScript : MonoBehaviour
{
    
    [SerializeField] OVRHand thisHand;
    bool handsDetected = false;

    private Coroutine personalUICoroutine;

    private void Start()
    {
        StartCoroutine(CheckIfHandsDetected());
    }

    ///<summary>
    ///continuously running Coroutine that checks if hands are being detected and updates the handsDetected variable for use in script
    ///</summary>
    IEnumerator CheckIfHandsDetected(){
        float timePassedWhileHandsNotTracked = 0;
        while(true){
            if(!thisHand.IsTracked){
                timePassedWhileHandsNotTracked += Time.deltaTime;
                handsDetected = false;
            } else {
                timePassedWhileHandsNotTracked = 0;
                handsDetected = true;
            }

            if(timePassedWhileHandsNotTracked > 5f){
                ///This part could potentially be used to activate the controller model
                Debug.LogError("Couldn't detect hands, make sure lighting is sufficient and there is nothing occluding your hands");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Coroutine waits for a second before displaying personalUI
    /// Only displays if person continues to hold gesture
    /// </summary>
    IEnumerator PersonalSystemGesture(){
        ///here you could display some sort of loading progress
        yield return new WaitForSeconds(1f);
        ///if the gesture is not being held, don't show the menu
        if(!thisHand.IsSystemGestureInProgress) StopCoroutine(personalUICoroutine);
        ///code for enabling and showing Menu
        GameObject.FindGameObjectsWithTag("PersonalUI").ToList().ForEach((g) => {g.GetComponent<Canvas>().enabled = true;});
        yield return null;
    }
}
