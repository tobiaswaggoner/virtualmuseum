using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Oculus.Interaction.Grab;

public class CSVInterpreter : MonoBehaviour
{
    public TextAsset inputText;
    private string[] data;
    public List<String> ersterwähnungen = new List<string>();
    public List<String> ort = new List<string>();
    public List<String> landkreis = new List<string>();
    private List<String> gPSOld = new List<string>();
    public List<String[]> gPS = new List<string[]>();

    // Start is called before the first frame update
    void Start()
    {
        data = inputText.text.Split(new String[] {";" , "\n"}, System.StringSplitOptions.None);
        for(int i = 0; i < data.Length; i ++){
            int j = i + 1; 
            if(j % 2 == 0 && !(j % 4 == 0)){
                ort.Add(data[i]);
            } else if(j % 3 == 0){
                landkreis.Add(data[i]);
            } else if(j % 4 == 0){
                gPSOld.Add(data[i]);
            } else {
                ersterwähnungen.Add(data[i]);
            }
        }

        for(int i = 0; i < gPS.Count; i ++){
            gPS[i] = new string[2];
        }
        List<string> temp = new List<string>();
        foreach(string s in gPSOld){
            var t = (s.Split(new string[] {","}, StringSplitOptions.None));
            temp.Add(t[0]);
            temp.Add(t[1]);
        }
        for(int i = 0; i < temp.Count - 1){
            var t = new String[2];
            t[0] = temp[i];
            t[1] = temp[i + 1];
            gPS.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
