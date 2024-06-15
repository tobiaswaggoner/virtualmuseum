using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TableSpawn : MonoBehaviour
{
    private const string PlayerPrefsKey = "GuidList";
    
    [SerializeField] private GameObject table;
    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private OVRCameraRig cameraRig;
    private Guid anchorGuid;
    [SerializeField] private string debugGuid;
    private OVRSpatialAnchor anchor;
    private List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors;
    private TableGhost tableGhostScript;

    private void Start()
    {
        tableGhostScript = GetComponent<TableGhost>();
        unboundAnchors = new List<OVRSpatialAnchor.UnboundAnchor>();
    }

    public void SpawnTableOnAnchor()
    {
        var anchorInScene = GameObject.FindWithTag("Anchor");
        if (!anchorInScene)
        {
            Debug.Log("No Spatial Anchor found in the scene!");
            return;
        }

        Instantiate(table, new Vector3(anchor.transform.position.x, 0f, anchor.transform.position.z), Quaternion.Euler(0, anchor.transform.rotation.eulerAngles.y, 0));
        gameObject.SetActive(false);
    }

    public void CreateSpatialAnchor()
    {
        if (!tableGhostScript.ghost) return;

        if (anchor != null)
        {
            Destroy(anchor.gameObject);
            anchor = null;
        }
        
        var anchorSpawn = Instantiate(anchorPrefab, tableGhostScript.GetGhostPosition(),
            tableGhostScript.GetGhostRotation());
        Debug.Log("Initialization process started!");
       StartCoroutine(AnchorCreation(anchorSpawn.AddComponent<OVRSpatialAnchor>()));
    }

    public void LoadSpatialAnchor()
    {
        var g = LoadGuid()[0];
        anchorGuid = g;
        LoadAnchorByUuid(new List<Guid>(){anchorGuid});
    }

    private IEnumerator AnchorCreation(OVRSpatialAnchor ovrAnchor)
    {
        while (!ovrAnchor.Created && !ovrAnchor.Localized)
        {
            yield return null;
        }

        if (anchorGuid != Guid.Empty)
        {
            var erase = AnchorErase(new List<Guid>(){anchorGuid});
            while (!erase.IsCompleted) yield return null;
        }

        Debug.Log("Trying to save anchor!");
        var save = AnchorSave(ovrAnchor);
        float saveTimeout = Time.time + 5f; // 5-second timeout for save operation
        while (!save.IsCompleted)
        {
            if (Time.time > saveTimeout)
            {
                Debug.Log("Saving anchor timed out.");
                yield break;
            }
            yield return null;
        }

        if (save.Status == TaskStatus.RanToCompletion)
        {
            anchor = ovrAnchor;
        }
    }

    private async Task AnchorSave(OVRSpatialAnchor a)
    {
        var result = await a.SaveAnchorAsync();
        if (result.Success)
        {
            Debug.Log($"Saved {a.Uuid}"!);
            anchorGuid = a.Uuid;
            SaveGuid(anchorGuid);
        }
        else
        {
            Debug.Log($"Failed to save {a.Uuid}!: {result.Status}");
        }
    }

    private async Task AnchorErase(List<Guid> uuids)
    {
        var result = await OVRSpatialAnchor.EraseAnchorsAsync(null, uuids);
        if (result.Success)
        {
            Debug.Log($"Erased {string.Join(", ", uuids.Select(guid => guid.ToString()).ToArray())}"!);
            anchorGuid = Guid.Empty;
        }
        else
        {
            Debug.LogError($"Failed to erase {uuids}!: {result.Status}");
        }
    }

    private async void LoadAnchorByUuid(IEnumerable<Guid> uuids)
    {
        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(uuids, unboundAnchors);

        if (result.Success)
        {
            Debug.Log($"Loaded Anchors.");

            foreach (var unboundAnchor in result.Value)
            {
                unboundAnchor.LocalizeAsync().ContinueWith((success, a) =>
                {
                    if (success)
                    {
                        // Create a new game object with an OVRSpatialAnchor component
                        var spatialAnchor = Instantiate(anchorPrefab).AddComponent<OVRSpatialAnchor>();

                        // Step 3: Bind
                        // Because the anchor has already been localized, BindTo will set the
                        // transform component immediately.
                        unboundAnchor.BindTo(spatialAnchor);
                        anchor = spatialAnchor;
                    }
                    else
                    {
                        Debug.LogError($"Localization failed for anchor {unboundAnchor.Uuid}");
                    }
                }, unboundAnchor);
            }
        }
        else
        {
            Debug.Log($"Failed to load anchors: {result.Status}");
        }
    }

    public void SaveGuid(Guid guid)
    {
        PlayerPrefs.SetString(PlayerPrefsKey, guid.ToString());
        PlayerPrefs.Save();

        Debug.Log("Guid saved successfully.");
    }

    public List<Guid> LoadGuid()
    {
        // Get the string from PlayerPrefs
        string guidString = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
        Debug.Log($"Read PlayerPrefs! GUID: {guidString}");

        // If the string is empty, return a new list
        if (string.IsNullOrEmpty(guidString))
        {
            Debug.Log("No Guid found, returning a new list.");
            anchorGuid = Guid.Empty;
            return new List<Guid>();
        }
        
        var guidList = new List<Guid>();
        debugGuid = guidString;
        guidList.Add(Guid.Parse(guidString));

        Debug.Log("Guid loaded successfully.");
        return guidList; 
    }
}
