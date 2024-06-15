using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PokeEventInterpreter : MonoBehaviour
{
    private UnityEvent<bool> pokedEvent = new UnityEvent<bool>();

    public void RegisterForPokedEvent(UnityAction<bool> pokedAction){
        pokedEvent.AddListener(pokedAction);
    }
    public void UnregisterForPokedEvent(UnityAction<bool> pokedAction){
        pokedEvent.RemoveListener(pokedAction);
    }

    public void Selected(){
        pokedEvent.Invoke(true);
    }

    public void Unselected(){
        pokedEvent.Invoke(false);
    }
}
