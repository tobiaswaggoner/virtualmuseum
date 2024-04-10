using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformScript : MonoBehaviour
{
    [SerializeField] private Transform transformToFollow;
    public bool isFollowing = false;

    public void SetTransformToFollow(Transform t) {transformToFollow = t;}

    // Update is called once per frame
    void Update()
    {
        if(isFollowing){
            if(transformToFollow.Equals(null)) return;
            transform.position = Vector3.MoveTowards(transform.position
            , transformToFollow.position
            , (transformToFollow.position - transform.position).magnitude / 3f);
        }
        
    }
}
