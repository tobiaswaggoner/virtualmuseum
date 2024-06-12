using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TableSpawn : MonoBehaviour
{
    private const string PlayerPrefsKey = "GuidList";
    
    [SerializeField] private GameObject table;
    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private OVRCameraRig cameraRig;
    [SerializeField] private List<Guid> anchorGuids;
    private OVRSpatialAnchor anchor;
    private List<OVRSpatialAnchor.UnboundAnchor> _unboundAnchors = new();

    private void Start()
    {
        anchorGuids = LoadGuidList();
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
        if (anchor != null) return;
        
        var anchorSpawn = Instantiate(anchorPrefab, cameraRig.rightControllerInHandAnchor.position,
            Quaternion.Euler(0, cameraRig.rightControllerInHandAnchor.rotation.eulerAngles.y, 0));
        Debug.Log("Initialization process started!");
       StartCoroutine(AnchorCreation(anchorSpawn.AddComponent<OVRSpatialAnchor>()));
    }

    public void LoadSpatialAnchor()
    {
        if (anchorGuids.Count == 0)
        {
            Debug.Log("No Anchors saved!");
        }
        
        LoadAnchorByUuid(anchorGuids);
    }

    private IEnumerator AnchorCreation(OVRSpatialAnchor ovrAnchor)
    {
        while (!ovrAnchor.Created && !ovrAnchor.Localized)
        {
            yield return null;
        }

        if (anchorGuids.Count > 0)
        {
            var erase = AnchorErase(anchorGuids);
            while (!erase.IsCompleted) yield return null;
            Debug.Log("Erased!");
        }

        Debug.Log("Trying to save anchor!");
        AnchorSave(ovrAnchor);
        anchor = ovrAnchor;
    }

    private async Task AnchorSave(OVRSpatialAnchor a)
    {
        var result = await a.SaveAnchorAsync();
        if (result.Success)
        {
            Debug.Log($"Saved {a.Uuid}"!);
            anchorGuids.Add(a.Uuid);
            SaveGuidList(anchorGuids);
        }
        else
        {
            Debug.LogError($"Failed to save {a.Uuid}!: {result.Status}");
        }
    }

    private async Task AnchorErase(List<Guid> uuids)
    {
        var result = await OVRSpatialAnchor.EraseAnchorsAsync(null, uuids);
        if (result.Success)
        {
            Debug.Log($"Erased {uuids}"!);
            anchorGuids.Clear();
        }
        else
        {
            Debug.LogError($"Failed to erase {uuids}!: {result.Status}");
        }
    }

    private async void LoadAnchorByUuid(IEnumerable<Guid> uuids)
    {
        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(uuids, _unboundAnchors);

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

    public void SaveGuidList(List<Guid> guidList)
    {
        // Convert the list of Guids to a single string, with each Guid separated by a delimiter (e.g., ',')
        string guidString = string.Join(",", guidList.ConvertAll(g => g.ToString()).ToArray());

        // Save the string in PlayerPrefs
        PlayerPrefs.SetString(PlayerPrefsKey, guidString);
        PlayerPrefs.Save();

        Debug.Log("Guid list saved successfully.");
    }

    public List<Guid> LoadGuidList()
    {
        // Get the string from PlayerPrefs
        string guidString = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);

        // If the string is empty, return a new list
        if (string.IsNullOrEmpty(guidString))
        {
            Debug.LogWarning("No Guid list found, returning a new list.");
            return new List<Guid>();
        }

        try
        {
            // Split the string into an array of Guid strings
            string[] guidStrings = guidString.Split(',');

            // Convert each Guid string back into a Guid object
            List<Guid> guidList = new List<Guid>();
            foreach (string guidStr in guidStrings)
            {
                guidList.Add(Guid.Parse(guidStr));
            }

            Debug.Log("Guid list loaded successfully.");
            return guidList;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load guid list: " + e.Message);
            return new List<Guid>();
        }
    }
}
