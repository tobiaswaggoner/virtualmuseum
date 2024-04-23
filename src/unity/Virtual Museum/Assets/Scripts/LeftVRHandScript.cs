using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static OVRHand;

public class LeftVRHandScript : MonoBehaviour
{
    private InputListener inputListener;
    [SerializeField] CSVInterpreter cSVInterpreter;
    [SerializeField] OVRHand thisHand;
    [SerializeField] private LineRenderer lineRenderer;
    bool handsDetected = false;
    bool wasPinching = false;

    private Coroutine personalUICoroutine;
    // TEST ----------------------
    int testPeriod = 704;
    // TEST ----------------------

    private void Start()
    {
        inputListener = GameObject.Find("InputsAndLogic").GetComponent<InputListener>();
    }

    float timer = 0;
    void Update()
    {
        if(!handsDetected) return;
        timer += Time.deltaTime;
        if(thisHand.IsSystemGestureInProgress){
            if(!thisHand.IsDominantHand){
                ///display the next period
                if(timer > 0.4){
                    cSVInterpreter.DisplayFromPeriod(testPeriod);
                    testPeriod = cSVInterpreter.getNextPeriod(testPeriod);
                    timer = 0;
                }
                
                if(personalUICoroutine.Equals(null)){
                    ///Coroutine activates PersonalMenu after a second
                    // personalUICoroutine = StartCoroutine(PersonalSystemGesture());
                }
            }
        } else {
            DetectPinch(); 
        }
    }

    


    ///------------------------------------------------------------------------------------------------------------------------------------
    ///helper functions and debugging -----------------------------------------------------------------------------------------------------
    ///------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Tries to detect if this hand is pinching and displays Rays according to strength of the pinch
    /// </summary>
    /// <returns> The current index tip position if pinch strength is bigger than 0.9f </returns>

    private StandardFlag selectedMarker;
    private void DetectPinch()
    {
        bool isIndexFingerPinching = thisHand.GetFingerIsPinching(HandFinger.Index);
        TrackingConfidence confidence = thisHand.GetFingerConfidence(HandFinger.Index);

        if (!isIndexFingerPinching)
        {
            // Pinch released?
            if (wasPinching)
            {
                if(!selectedMarker.Equals(null)){
                    //sets the 
                    selectedMarker.ShowText();
                }
            }

            wasPinching = false;
            lineRenderer.enabled = false;
            return;
        }

        if (isIndexFingerPinching && confidence == TrackingConfidence.High)
        {
            wasPinching = true;
            //Code upon pinch gesture start
           if (inputListener.sessionState != InputListener.SessionState.ToolPlacement)
            {
                /*
                CreateMarker();
                */

                Ray ray = new Ray(thisHand.PointerPose.position, thisHand.PointerPose.forward);
                Physics.Raycast(ray, out RaycastHit hit);
                Color color = Color.gray;
                var test = hit.transform;
                if(!test.Equals(null)){
                    if(hit.transform.CompareTag("Marker")){
                        if(!selectedMarker.Equals(null)){
                            selectedMarker.HideText();
                        }
                        
                        color = Color.green;
                        selectedMarker = hit.transform.GetComponent<StandardFlag>();
                        selectedMarker.Activate();
                        DisplayRayFromPinchPosition(thisHand.PointerPose, color);
                    }
                }
            }
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
