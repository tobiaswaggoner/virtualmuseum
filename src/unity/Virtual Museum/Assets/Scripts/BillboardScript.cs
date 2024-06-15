using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    Transform cameraTransform;
    /// Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.gameObject.transform;
    }

    /// Update is called once per frame
    void Update()
    {
        if(cameraTransform.Equals(null)) return;
        Vector3 pointToLookAt = transform.position - (cameraTransform.position - transform.position);
        transform.LookAt(pointToLookAt);
    }
}
