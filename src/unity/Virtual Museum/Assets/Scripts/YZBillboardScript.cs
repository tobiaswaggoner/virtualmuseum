using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YZBillboardScript : MonoBehaviour
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
        var position = transform.position;
        var pointToLookAt = position - (cameraTransform.position - position);
        pointToLookAt.y = position.y;
        transform.LookAt(pointToLookAt);
    }
}
