using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AnimateOnAwake : MonoBehaviour
{
    static List<AudioSource> audioSources = new List<AudioSource>();
    public bool animateOnAwake = true;
    public ParticleSystem _particleSystem;
    // Start is called before the first frame update
    void Start(){
        audioSources = new List<AudioSource>();
    }
    private void OnEnable(){
        
        if(animateOnAwake){
            Debug.Log("huh");
            transform.localScale = Vector3.zero;
            if(_particleSystem){
                _particleSystem.Play();
            }
            AudioSource audioSource = GetComponent<AudioSource>();
            if(!audioSources.Contains(audioSource)){
                audioSources.Add(audioSource);
            } 
            bool isPlaying = false;
            foreach(var a in audioSources){
                if(a.isPlaying){
                    isPlaying = true;
                    break;
                }
            }
            if(!isPlaying)audioSource.Play();
            LeanTween.scale(gameObject, Vector3.one * 0.01f, 2f).setEase(LeanTweenType.easeOutBounce).setOnComplete(StopParticleSystem);
        }
    }

    private void StopParticleSystem(){
        if(_particleSystem){
            _particleSystem.Stop();
        }
    }

    
}
