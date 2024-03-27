using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    /*Script for managing game state and interpreting input accordingly*/
    [SerializeField] private List<GameObject> UserInterfaces;

    [SerializeField] private List<GameObject> Tools;
    [SerializeField] private List<GameObject> ToolGhosts;
    private bool ghostSpawned = false;
    private GameObject currentGhost;
    private int toolIndex;
    private bool placedTool = false;
    private Vector3 placementPosition;
    private Quaternion placementRotation = Quaternion.identity;

    private bool menuActive = false;
    enum SessionState{
        Selection,
        SpaceDefinition,
        ToolPlacement,
        Interaction
    }

    private SessionState sessionState = SessionState.ToolPlacement;
    public void ActivateMenu(InputAction.CallbackContext context){

        //Check if a menu is already active, if it is deactivate the current menu
        //If it isn't, activate a menu according to current sessionState
        if(menuActive){
            UserInterfaces[(int) sessionState].SetActive(false);
            menuActive = false;
            return;
        }

        UserInterfaces[(int) sessionState].SetActive(true);
        menuActive = true;
    }
    
    public void TriggerRight(InputAction.CallbackContext context){
        switch (sessionState){
            case SessionState.Selection:
                break;
            case SessionState.SpaceDefinition:
                break;
            case SessionState.ToolPlacement:
                //Check if tool has been placed
                if(placedTool) {
                    sessionState = SessionState.Interaction;
                    Debug.LogError("Sessionstate was in 'ToolPlacement' when tool was already placed \n Changed state to 'Interaction'");
                    return;
                }
                //If tool hasn't been placed, place tool at location and rotation of selection
                var newTool = Instantiate(Tools[toolIndex]);
                newTool.transform.position = placementPosition;
                newTool.transform.rotation = placementRotation;
                placedTool = true;
                

                //Deactivate current menu
                UserInterfaces[(int) sessionState].SetActive(false);
                menuActive = false;
                //Switch sessionState over
                sessionState = SessionState.Interaction;
                //Deactivate and de-spawn Ghost;
                Destroy(currentGhost);
                ghostSpawned = false;
                return;
            case SessionState.Interaction:
                break;
            default: return;
        }
        if(sessionState.Equals(SessionState.ToolPlacement) && !placedTool){
            
        }
    }

    void Update()
    {
        if(sessionState.Equals(SessionState.ToolPlacement) && menuActive){
            if(!ghostSpawned) {
                currentGhost = Instantiate(ToolGhosts[toolIndex]);
                ghostSpawned = true;
            }
            if(currentGhost.activeSelf) currentGhost.SetActive(true);
            //Cast a ray from the dominant VR hand (test case = right)
            //For testing on PC cast ray through mousePoint
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out  hit)){
                currentGhost.transform.position = hit.point;
                placementPosition = hit.point;
            }
        }
    }

}
