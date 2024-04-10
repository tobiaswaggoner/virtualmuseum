using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<summary>
///script for smoothly sliding a UI in front of the User at a certain distance
///</summary>
public class PersonalUIScript : MonoBehaviour
{
    [SerializeField] GameObject playerHead;

    private Vector3 targetPosition;
    [SerializeField] private float distanceToHead = 3f;

    float passedTime = 0f;

    /// Update is called once per frame
    void Update()
    {
        targetPosition = playerHead.transform.position + playerHead.transform.forward * distanceToHead;
        targetPosition.y = transform.position.y;
        passedTime += Time.deltaTime;
        if(passedTime >= 2f){
            StartCoroutine(AdjustMenu());
            passedTime = 0;
        }
    }

    IEnumerator AdjustMenu(){
        float t = 0;
        Vector3 startPosition =  transform.position;
        while (t <= 1){
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            t += Time.deltaTime;
        }
        yield return null;
    }

    public void DeactivatePersonalMenu(){
        GetComponent<Canvas>().enabled = false;
    }
}
