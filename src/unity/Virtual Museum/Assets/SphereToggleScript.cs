using System.Collections;
using UnityEngine;

public class SphereToggleScript : MonoBehaviour
{
    // This sphere will becom visible every 5 seconds
    // and invisible every 5 seconds
    
    // Start is called before the first frame update
    private MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(ToggleSphere());
    }

    IEnumerator ToggleSphere(){
        while(true){
            yield return new WaitForSeconds(5f);
            meshRenderer.enabled = !meshRenderer.enabled;
        }
    }
}
