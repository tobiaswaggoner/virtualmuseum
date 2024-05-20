using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Timeline;

///A standard flag for displaying information about a point of interest
///Has start and endTime for defining first mention and last seen documentation of the point of interest as well as Color attribute for 
///categorization and visual clarity
public class StandardFlag : IFlag
{
    public int startTime { get; set; }
    static public int currentTime = 700; //704 is the date of the first city
    public static List<StandardFlag> flags = new List<StandardFlag>();
    public Transform transform {get; set;}
    public Vector3 position { get; set; }
    public GameObject flagVisualTextComponent { get; set; }
    public GameObject flagVisualIndicator { get; set; }
    public string header { get; set; }
    public string info { get; set; }
    Color flagColor;
    public Transform visualComponentTransform;
    public Transform textTransform;
    public PokeEventInterpreter pokeEventInterpreter;
    private UnityAction<bool> pokedListener;

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
        this.pokeEventInterpreter = transform.GetComponent<PokeEventInterpreter>();
        pokedListener = new UnityAction<bool>(EventCallback);
        pokeEventInterpreter.RegisterForPokedEvent(pokedListener);
        flags.Add(this);
        this.Deactivate();
    }

    static public bool NextPeriod(){
        //nullable variable for storing potential next period
        int? nextTime = flags
            .Where(flag => flag.startTime > currentTime)
            .Select(flag => (int?)flag.startTime)
            .OrderBy(time => time)
            .FirstOrDefault();

        if(nextTime.HasValue){
            currentTime = nextTime.Value;
            DisplayMarkersOfPeriod(currentTime);
            return true;
        }
        return false;
    }

    static public bool LastPeriod(){
        //nullable variable for storing potential next period
        int? lastTime = flags
            .Where(flag => flag.startTime < currentTime)
            .Select(flag => (int?)flag.startTime)
            .OrderByDescending(time => time)
            .FirstOrDefault();

        if(lastTime.HasValue){
            currentTime = lastTime.Value;
            DisplayMarkersOfPeriod(currentTime);
            return true;
        }
        return false;
    }

    static public void DisplayMarkersOfPeriod(int period){
        flags
        .Where(flag => flag.startTime == period)
        .ToList()
        .ForEach(flag => flag.Activate());

        flags.ForEach(flag => flag.UpdateColor());
    }

    //Change color according to distance between currentTime and startTime
    public void UpdateColor(){
        //Flag should be more red when fresher
        float r = 255 - (startTime - currentTime) / 4;
        float g = 0;
        float b = (startTime - currentTime) / 4;
        Color newColor = new Color(r,g,b);
        visualComponentTransform.GetChild(0).GetComponent<MeshRenderer>().material.color = newColor;
        flagColor = newColor;
        }

    private void EventCallback(bool b){
        if(b){
            ShowText();
        } else {
            HideText();
        }
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
