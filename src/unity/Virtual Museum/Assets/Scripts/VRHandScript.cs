using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static OVRHand;

public class VRHandScript : MonoBehaviour
{
    //TEMPORARY VARIABLES FOR TESTING PURPOSES
    [SerializeField] private GameObject markerPrefab;
    private bool holdingMarker = false;
    private GameObject currentMarker;
    private FollowTransformScript followTransformScript;
    //TEMPORARY VARIABLES FOR TESTING PURPOSES

    private InputListener inputListener;
    [SerializeField] OVRHand thisHand;
    [SerializeField] private LineRenderer lineRenderer;
    bool handsDetected = false;
    bool wasPinching = false;

    private Coroutine personalUICoroutine;

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
                ///code for enabling and showing Menu
                if(personalUICoroutine.Equals(null)){
                    ///Coroutine activates PersonalMenu after a second
                    // personalUICoroutine = StartCoroutine(PersonalSystemGesture());
                }
            }
        } else {
            DetectPinch(); 
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
            yield return new WaitForSeconds(1f);
        }
    }


    ///------------------------------------------------------------------------------------------------------------------------------------
    ///helper functions and debugging -----------------------------------------------------------------------------------------------------
    ///------------------------------------------------------------------------------------------------------------------------------------

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
            if (wasPinching)
            {
                if (inputListener.sessionState == InputListener.SessionState.ToolPlacement)
                {
                    // Set the table
                    inputListener.PlaceTool();
                }
                else
                {
                    //Release the Marker;
                    ReleaseMarker();
                }
            }

            wasPinching = false;
            lineRenderer.enabled = false;
            return;
        }

        if (isIndexFingerPinching && confidence == TrackingConfidence.High)
        {
            {
                wasPinching = true;

                if (inputListener.sessionState != InputListener.SessionState.ToolPlacement && !holdingMarker)
                {
                    CreateMarker();
                }
                else if (inputListener.sessionState == InputListener.SessionState.ToolPlacement)
                {
                    MoveTable();
                }
            }
        }
    }

    private void MoveTable()
    {
        var pinchTransform = thisHand.PointerPose;
        var ray = new Ray(pinchTransform.position, pinchTransform.forward);
        Physics.Raycast(ray, out RaycastHit hit);

        Color color;

        if (hit.transform.gameObject.name == "TestFloor")
        {
            color = Color.green;
            inputListener.ActivateGhost();
            inputListener.UpdateGhostPosition(hit.point);
        }
        else
        {
            color = Color.gray;
            inputListener.DeactivateGhost();
        }

        DisplayRayFromPinchPosition(thisHand.PointerPose, color);
    }

    private void CreateMarker()
    {
        if (currentMarker is not null || holdingMarker) return;

        currentMarker = Instantiate(markerPrefab);
        currentMarker.GetComponent<Rigidbody>().isKinematic = true;

        followTransformScript = currentMarker.GetComponent<FollowTransformScript>();
        followTransformScript.SetTransformToFollow(thisHand.PointerPose);
        followTransformScript.isFollowing = true;

        holdingMarker = true;
    }

    private void ReleaseMarker()
    {
        if (currentMarker is null || holdingMarker==false) return;
        followTransformScript = currentMarker.GetComponent<FollowTransformScript>();
        followTransformScript.isFollowing = false;
        currentMarker.GetComponent<Rigidbody>().isKinematic = false;
        currentMarker = null;
        holdingMarker= false;
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
