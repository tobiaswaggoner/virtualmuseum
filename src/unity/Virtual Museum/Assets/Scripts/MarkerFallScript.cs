using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerFallScript : MonoBehaviour
{
    void Update()
    {
        if(transform.position.y < 0){
            Destroy(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Table")){
            GetComponent<Rigidbody>().isKinematic = true;
            //potentially smooth deceleration
        }
    }
}
