using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateMarkerVisual : MonoBehaviour
{
    public List<GameObject> viusals = new List<GameObject>();
    public Transform housePosition;
    // Start is called before the first frame update

    public void InstantiateMarker()
    {
        Debug.Log("instantiating marker");
        int index = Random.Range(0, viusals.Count);
        GameObject viusal = Instantiate(viusals[index], housePosition.position, Quaternion.identity);
        housePosition.gameObject.SetActive(true);
        viusal.transform.parent = housePosition;
        viusal.transform.localScale = Vector3.one;
        viusal.transform.localPosition = Vector3.zero;
        //randomize rotation of visual by rotating randomly between -90 and 90 degrees along the y axis
        viusal.transform.localRotation = Quaternion.Euler(0, Random.Range(-90, 90), 0);
    }

}
