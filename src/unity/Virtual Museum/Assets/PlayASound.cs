using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayASound : MonoBehaviour
{
    // Assign this in the Inspector or through another script
    public AudioSource audioSource;

    private void Start()
    {
        // URL of the .mp3 file
        string url = "http://localhost:3000/test?question=\"Tell%20me%20a%20knock-knock%20joke\"";

        // Start the coroutine to download and play the audio
        StartCoroutine(DownloadAndPlay(url));
    }

    IEnumerator DownloadAndPlay(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            // Send the request
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                // Handle errors
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Retrieve the audio clip from the web request
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                if (clip == null)
                {
                    Debug.LogError("Failed to download audio clip.");
                    yield break;
                }

                // Assign the audio clip to the AudioSource
                audioSource.clip = clip;

                // Play the audio clip
                audioSource.Play();
            }
        }
    }
}