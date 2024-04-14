using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OVR.OpenVR;
using UnityEngine;

public class TableCoordinates : MonoBehaviour
{
    private CSVInterpreter cSVInterpreter;
    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(5000);
        var temp = GetComponent<MeshFilter>();
        Vector3[] verts = temp.mesh.vertices;
        Vector4 globalOffsetToTable = transform.parent.position;
        for(int i = 0; i < verts.Length; i ++){
            var v = transform.localToWorldMatrix * verts[i] + globalOffsetToTable;
            verts[i] = v;
            verts[i].y = transform.position.y;
        }
        cSVInterpreter = GameObject.FindGameObjectWithTag("InputsAndLogic").GetComponent<CSVInterpreter>();
        cSVInterpreter.UpdateDesiredCorners(verts[0], verts[verts.Length - 1]);
        await cSVInterpreter.CalculateStuff();
        
    }
}
