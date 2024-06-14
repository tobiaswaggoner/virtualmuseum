using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    static int counter = 0;
    private void Start() {
        counter = 0;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(int.Parse(gameObject.transform.name) == 704 && counter == 0){
                DisplayBlock();
                counter ++;
            } else if(int.Parse(gameObject.transform.name) == 774 && counter == 1){
                DisplayBlock();
                counter ++;
            }
        }
    }

    public void AdvanceTime(){
        StandardFlag.NextPeriod();
    }

    public void DecreaseTime(){
        StandardFlag.LastPeriod();
    }

    public void DisplayBlock(){
        Debug.Log("DisplayBlock: " + gameObject.transform.name);
        transform.parent.parent.GetComponent<PersonalUIManager>().ActivateCityList();
        StandardFlag.DisplayBlock(int.Parse(gameObject.transform.name));
        
    }

    public void DisplayInformation(){
        
    }
}
