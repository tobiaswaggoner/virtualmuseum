using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static OVRHand;

public class VRHandScript : MonoBehaviour
{
    OVRHand thisHand;
    OVRSkeleton skeleton;
    [SerializeField] private LineRenderer lineRenderer;
    bool handsDetected = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        thisHand = GetComponent<OVRHand>();
        skeleton = GetComponent<OVRSkeleton>();
        StartCoroutine(CheckIfHandsDetected());

        if(DetectPinch() && handsDetected && !thisHand.IsSystemGestureInProgress){
            if(thisHand.IsPointerPoseValid){
                Transform handTipPos;
                //Interaction with UI
                if(!lineRenderer){
                    throw new NotImplementedException("No line renderer assigned to this Object: " + gameObject.name);
                }
                foreach(var b in skeleton.Bones){
                    if(b.Id.Equals(OVRSkeleton.BoneId.Hand_IndexTip)){
                        handTipPos = b.Transform;
                        lineRenderer.enabled = true;
                        lineRenderer.SetPosition(0, handTipPos.position);
                        lineRenderer.SetPosition(1, handTipPos.position + handTipPos.forward * 5);
                    }
                }
            }
            throw new NotImplementedException("Pinch detected, but method not implemented yet");
        } else {
            if(!lineRenderer){
                throw new NotImplementedException("No line renderer assigned to this Object: " + gameObject.name);
            }
            lineRenderer.enabled = false;
        }
    }

    

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
                //This part could potentially be used to activate the controller model
                Debug.LogError("Couldn't detect hands, make sure lighting is sufficient and there is nothing occluding your hands");
            }
        }
    }

    private bool DetectPinch(){
        bool isIndexFingerPinching = thisHand.GetFingerIsPinching(HandFinger.Index);
        float indexFingerPinchStrength = thisHand.GetFingerPinchStrength(HandFinger.Index);
        TrackingConfidence confidence = thisHand.GetFingerConfidence(HandFinger.Index);

        if(isIndexFingerPinching == true && confidence == TrackingConfidence.High && indexFingerPinchStrength > 0.8f){
            return true;
        }
        return false;
    }
}
