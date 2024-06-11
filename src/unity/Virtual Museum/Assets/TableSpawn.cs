using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSpawn : MonoBehaviour
{
    [SerializeField] private GameObject table;
    public void SpawnTableOnAnchor()
    {
        var anchor = GameObject.FindWithTag("Anchor");
        if (!anchor)
        {
            Debug.Log("No Spatial Anchor found in the scene!");
            return;
        }

        Instantiate(table, anchor.transform.position, Quaternion.Euler(0, anchor.transform.rotation.eulerAngles.y, 0));
        gameObject.SetActive(false);
    }
}
