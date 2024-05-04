using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;
using TMPro;
using UnityEngine;

///A standard flag for displaying information about a point of interest
///Has start and endTime for defining first mention and last seen documentation of the point of interest as well as Color attribute for 
///categorization and visual clarity
public class StandardFlag : IFlag
{
    public int startTime { get; set; }
    public Transform transform {get; set;}
    public Vector3 position { get; set; }
    public GameObject flagVisualTextComponent { get; set; }
    public GameObject flagVisualIndicator { get; set; }
    public string header { get; set; }
    public string info { get; set; }
    Color flagColor;

    private bool textIsSet = false;

    public StandardFlag(int startTime, Vector3 position, Transform transform, Color flagColor, string header = "test", string info = "no new info"){
        this.startTime = startTime;
        this.transform = transform;
        this.position = position;
        this.header = header;
        this.info = info;
        this.flagColor = flagColor;
    }

    public void SetColor(Color color){
        transform.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        flagColor = color;
    }

    public void Activate(){
        //wait for animation and play sound
        Animator animator = transform.GetComponent<Animator>();
        AudioSource audioSource = transform.GetComponent<AudioSource>();
        audioSource.Play();
        animator.Play("Base Layer.MarkerAnim", 0, 0);
        transform.GetComponent<MeshRenderer>().enabled = true;
        transform.GetComponent<Collider>().enabled = true;
        SetColor(Color.red);
    }

    public void Deactivate(){
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<Collider>().enabled = false;
    }

    public void ShowText(){
        if(!textIsSet) SetText();
        var textTransform = transform.GetChild(0); 
        if(!textTransform.Equals(null)){
            textTransform.gameObject.SetActive(true);
        }
    }

    public void HideText(){
        var textTransform = transform.GetChild(0); 
        if(!textTransform.Equals(null)){
            textTransform.gameObject.SetActive(false);
        }
    }

    private void SetText(){
        transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = header;
        textIsSet = true;
    }
}
