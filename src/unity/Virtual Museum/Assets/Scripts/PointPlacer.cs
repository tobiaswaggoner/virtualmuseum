using UnityEngine;

public class PointPlacer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 point = hit.point;
                if(Territory.selectedTerritory == null){
                    new Territory((int)Territory.currentTime, point, "testTerritory" + Territory.currentTime, "none", Color.red);
                    return;
                }
                Territory.selectedTerritory.AddPointToCurrentBorder(point, (int) Territory.currentTime, Color.blue);
            }
        } else if (Input.GetMouseButtonDown(1)){
            Territory.currentTime ++;
            Debug.Log("TerritoryTime: " + Territory.currentTime);
        }
    }
}