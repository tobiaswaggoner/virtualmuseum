using System;
using System.Collections;
using System.Linq;
using Meta.WitAi.Attributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR;
using static OVRHand;

public class VRHandScript : MonoBehaviour
{
    private InputListener inputListener;
    OVRHand thisHand;
    OVRSkeleton skeleton;
    [SerializeField] private LineRenderer lineRenderer;
    bool handsDetected = false;

    private Coroutine personalUICoroutine;

    private void Start()
    {
        inputListener = GameObject.Find("InputsAndLogic").GetComponent<InputListener>();
        thisHand = GetComponent<OVRHand>();
        skeleton = GetComponent<OVRSkeleton>();
        StartCoroutine(CheckIfHandsDetected());
        
    }

    void Update()
    {
        if(!handsDetected) return;
        if(thisHand.IsSystemGestureInProgress){
            if(!thisHand.IsDominantHand){
                ///code for enabling and showing Menu
                if(personalUICoroutine.Equals(null)){
                    ///Coroutine activates PersonalMenu after a second
                    personalUICoroutine = StartCoroutine(PersonalSystemGesture());
                }
            }
        } else {
            CheckForPinch();
        }
    }

    /// <summary>
    /// Handles interaction upon pinch. Currently places tool upon pinch
    /// </summary>
    private void CheckForPinch(){
        Transform pinchTransform = DetectPinch(); 
        if(pinchTransform.Equals(null)){    
            ///Interaction with UI -> Cast a ray and look for interaction
            Ray ray = new Ray(pinchTransform.position, pinchTransform.forward);
            Physics.Raycast(ray, out RaycastHit hit);

            if(hit.transform.GetComponent<Button>()){
                hit.transform.GetComponent<Button>().onClick.Invoke();
                return;
            }
            
            if(!inputListener.sessionState.Equals(InputListener.SessionState.ToolPlacement)) return;
            inputListener.UpdateGhostPosition(hit.point);
            inputListener.PlaceTool();
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
        }
    }
    

    ///------------------------------------------------------------------------------------------------------------------------------------
    ///helper functions and debugging -----------------------------------------------------------------------------------------------------
    ///------------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Tries to detect if this hand is pinching and displays Rays according to strength of the pinch
    /// </summary>
    /// <returns> The current index tip position if pinch strength is bigger than 0.9f </returns>
    private Transform DetectPinch(){
        bool isIndexFingerPinching = thisHand.GetFingerIsPinching(HandFinger.Index);
        float indexFingerPinchStrength = thisHand.GetFingerPinchStrength(HandFinger.Index);
        TrackingConfidence confidence = thisHand.GetFingerConfidence(HandFinger.Index);
        Transform indexTipTransform = null;

        foreach(var b in skeleton.Bones){
            if(b.Id.Equals(OVRSkeleton.BoneId.Hand_IndexTip)){
                indexTipTransform = b.Transform;
            }
        }

        if(isIndexFingerPinching == true && confidence == TrackingConfidence.High){
            if(indexFingerPinchStrength > 0.9f){
                ///Pinch has been detected and confirmed
                ///What happens now is defined in CheckForPinch Function
                DisplayRayFromPinchPosition(indexTipTransform, Color.green);
                if(thisHand.IsPointerPoseValid) return indexTipTransform;
            }

            if(indexFingerPinchStrength > 0.2f){
                DisplayRayFromPinchPosition(indexTipTransform, Color.red);
                ///Handle hovering over things here    
            } else {
                lineRenderer.enabled = false;
            }
        }
        return null;
    }

    /// <summary>
    /// Displays a ray from the given transforms position, also takes a color for defining the the color of the line.
    /// Color fades over distance
    /// </summary>
    /// <param name="pinchTransform"></param>
    /// <param name="c"></param>
    public void DisplayRayFromPinchPosition(Transform pinchTransform, Color c){
        if(!lineRenderer){
            throw new NotImplementedException("No line renderer assigned to this Object: " + gameObject.name);
        }

        lineRenderer.enabled = true;
        lineRenderer.startColor = c;
        c.a = 0.1f;
        lineRenderer.endColor = c;
        lineRenderer.SetPosition(0, pinchTransform.position);
        lineRenderer.SetPosition(1, pinchTransform.position + pinchTransform.forward * 5);
    }
}
