using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TableGhost : MonoBehaviour
{
    [SerializeField] private OVRCameraRig cameraRig;
    [SerializeField] private GameObject tableGhost;

    private GameObject tableInstance;
    public bool ghost;

    private Vector2 movementVector;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SpawnTableGhost()
    {
        if (ghost) return;
        tableInstance = Instantiate(tableGhost, cameraRig.rightControllerInHandAnchor.position, Quaternion.identity);
        tableInstance.transform.position =
            new Vector3(tableInstance.transform.position.x, 0f, tableInstance.transform.position.z);
        ghost = true;
    }

    public void DestroyTableGhost()
    {
        if (!ghost) return;
        Destroy(tableInstance);
        ghost = false;
    }

    public void MoveGhost(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ghost) return;
        var rightController = cameraRig.rightControllerInHandAnchor;
        tableInstance.transform.rotation =
            Quaternion.Euler(0f, rightController.rotation.eulerAngles.y, 0f);
        tableInstance.transform.Translate(new Vector3(movementVector.x, 0f, movementVector.y) * (Time.deltaTime * 2f));
    }

    public Vector3 GetGhostPosition()
    {
        return !tableInstance ? Vector3.zero : tableInstance.transform.position;
    }

    public Quaternion GetGhostRotation()
    {
        return !tableInstance ? Quaternion.identity : tableInstance.transform.rotation;
    }
}
