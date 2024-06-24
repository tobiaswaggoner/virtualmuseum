using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AnimateOnAwake : MonoBehaviour
{
    public bool animateOnAwake = true;
    public ParticleSystem _particleSystem;
    // Start is called before the first frame update
    private void OnEnable(){
        
        if(animateOnAwake){
            Debug.Log("huh");
            transform.localScale = Vector3.zero;
            if(_particleSystem){
                _particleSystem.Play();
            }
            LeanTween.scale(gameObject, Vector3.one * 0.01f, 2f).setEase(LeanTweenType.easeOutBounce).setOnComplete(StopParticleSystem);
        }
    }

    private void StopParticleSystem(){
        if(_particleSystem){
            _particleSystem.Stop();
        }
    }

    
}
