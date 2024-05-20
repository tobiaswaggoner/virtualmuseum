using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGestureTest : MonoBehaviour
{
    private bool start = false;
    private Vector3 startPosition;
    private Vector3 startingRight;
    public Transform leftHandTransform;

    public CSVInterpreter interpreter;

    float timer = 0;
    float stepSpeed = 2f;
    float stepDefaultSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space) && !start) {
            Debug.Log("started detecting movement");
            start = true;
            startPosition = leftHandTransform.position;
            startingRight = leftHandTransform.right;
        } else if(Input.GetKeyDown(KeyCode.Space) && start){
            Debug.Log("stopped detecting movement");

            start = false;
            stepSpeed = stepDefaultSpeed;
        }

        if(Input.GetKeyDown(KeyCode.F)){
            SelectFirstFlag();
        }

        if(start && timer >= stepSpeed){
            timer = 0;
            stepSpeed = stepSpeed <= 1f ? 1f : stepSpeed - 0.1f;

            var difference = leftHandTransform.position - startPosition;
            var distance = difference.magnitude;
            var direction = difference.normalized;

            //check if difference is in the right direction
            if(CalculateSimilarity(startingRight, direction) > 0.6 && distance > 0.5){
                StandardFlag.NextPeriod();
                leftHandTransform.GetComponent<MeshRenderer>().material.color = Color.red;
            } else if(CalculateSimilarity(startingRight, direction) > 0.4){leftHandTransform.GetComponent<MeshRenderer>().material.color = Color.gray;}

            if(CalculateSimilarity(startingRight, direction) < 0.4 && distance > 0.5){
                StandardFlag.LastPeriod();
                leftHandTransform.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }

        

        float CalculateSimilarity(Vector3 A, Vector3 B){
        // Normalize the vectors
        Vector3 normA = A.normalized;
        Vector3 normB = B.normalized;

        // Compute the dot product
        float dotProduct = Vector3.Dot(normA, normB);

        // Map the dot product to a [0,1] range
        return (dotProduct + 1) / 2;
        }

        void SelectFirstFlag(){
           interpreter.erscheinungsMap[interpreter.currentPeriod][0].pokeEventInterpreter.Selected();
        }
    }

}
