using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Oculus.Platform;
using Oculus.Platform.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Transform visualComponentTransform;
    public Transform textTransform;

    private bool textIsSet = false;

    public StandardFlag(int startTime, Vector3 position, Transform transform, Color flagColor, string header = "test", string info = "no new info"){
        this.startTime = startTime;
        this.transform = transform;
        this.position = position;
        this.header = header;
        this.info = info;
        this.flagColor = flagColor;
        this.visualComponentTransform = transform.GetChild(0);
        this.textTransform = transform.GetChild(1);
        this.Deactivate();
    }

    public void SetColor(Color color){
        visualComponentTransform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
        flagColor = color;
    }

    public void Activate(){
        //wait for animation and play sound
        visualComponentTransform.gameObject.SetActive(true);
        Animator animator = visualComponentTransform.GetComponent<Animator>();
        AudioSource audioSource = visualComponentTransform.GetComponent<AudioSource>();
        audioSource.Play();
        animator.Play("Base Layer.MarkerAnim", 0, 0);
        SetColor(Color.red);
    }

    public void Deactivate(){
        visualComponentTransform.gameObject.SetActive(false);
    }

    public void ShowText(){
        if(!textIsSet) SetText();
        textTransform.gameObject.SetActive(true);
    }

    public void HideText(){
        var textTransform = transform.GetChild(1); 
        if(!textTransform.Equals(null)){
            textTransform.gameObject.SetActive(false);
        }
    }

    private void SetText(){
        textTransform.GetChild(0).GetComponent<TMP_Text>().text = header;
        textIsSet = true;
    }
}
