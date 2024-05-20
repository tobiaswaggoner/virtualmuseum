using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static OVRHand;

public class VRHandScript : MonoBehaviour
{
    private InputListener inputListener;
    [SerializeField] OVRHand thisHand;
    [SerializeField] private LineRenderer lineRenderer;
    bool handsDetected = false;
    bool wasPinching = false;

    private void Start()
    {
        inputListener = GameObject.Find("InputsAndLogic").GetComponent<InputListener>();
        StartCoroutine(CheckIfHandsDetected());
    }

    void Update()
    {
        if(!handsDetected) return;
        if(thisHand.IsSystemGestureInProgress){
            if(!thisHand.IsDominantHand){
            }
        } else {
            DetectPinch(); 
        }
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


    ///------------------------------------------------------------------------------------------------------------------------------------
    ///helper functions and debugging -----------------------------------------------------------------------------------------------------
    ///------------------------------------------------------------------------------------------------------------------------------------

    StandardFlag selectedMarker;
    /// <summary>
    /// Tries to detect if this hand is pinching and displays Rays according to strength of the pinch
    /// </summary>
    /// <returns> The current index tip position if pinch strength is bigger than 0.9f </returns> 
    private void DetectPinch()
    {
        bool isIndexFingerPinching = thisHand.GetFingerIsPinching(HandFinger.Index);
        TrackingConfidence confidence = thisHand.GetFingerConfidence(HandFinger.Index);

        if (!isIndexFingerPinching)
        {
            // Pinch released?
            if (wasPinching && thisHand.IsDominantHand)
            {
                if (inputListener.sessionState == InputListener.SessionState.ToolPlacement)
                {
                    // Set the table
                    inputListener.PlaceTool();
                }
                else
                {
                    /*
                    //Release the Marker;
                    ReleaseMarker();
                    */
                }
            }

            wasPinching = false;
            lineRenderer.enabled = false;
            return;
        }

        if (isIndexFingerPinching && confidence == TrackingConfidence.High && thisHand.IsDominantHand)
        {
            wasPinching = true;
        }   
    }

    /// <summary>
    /// Displays a ray from the given transforms position, also takes a color for defining the the color of the line.
    /// Color fades over distance
    /// </summary>
    /// <param name="pinchTransform"></param>
    /// <param name="c"></param>
    public void DisplayRayFromPinchPosition(Transform pinchTransform, Color c)
    {
        if (!lineRenderer) return;

        lineRenderer.enabled = true;
        lineRenderer.startColor = c;
        c.a = 0.1f;
        lineRenderer.endColor = c;
        lineRenderer.SetPosition(0, pinchTransform.position);
        lineRenderer.SetPosition(1, pinchTransform.position + pinchTransform.forward * 5);
    }
}
