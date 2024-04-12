using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformScript : MonoBehaviour
{
    [SerializeField] private Transform transformToFollow;
    public bool isFollowing = false;

    public void SetTransformToFollow(Transform t) {
        transformToFollow = t;
        isFollowing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isFollowing || transformToFollow is null) return;
        transform.position = transformToFollow.position;
    }
}
