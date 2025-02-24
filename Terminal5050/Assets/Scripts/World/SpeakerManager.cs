using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SpeakerManager : MonoBehaviour
{
    public static SpeakerManager Instance;

    private List<Speaker> _speakers;

    public bool playing;

    private void Awake()
    {
        Instance = this;

        _speakers = FindObjectsOfType<Speaker>().ToList();
    }

    public void StopPlaying()
    {
        for (int i = 0; i < _speakers.Count; i++)
        {
            _speakers[i].StopPlaying();
        }

        playing = false;
    }

    public void StartPlaying(string source)
    {
        string path = Path.Combine(Application.persistentDataPath, source);
        string uri = "file://" + path;

        StartCoroutine(LoadAndPlay(uri));
    }

    IEnumerator LoadAndPlay(string uri)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                for (int i = 0; i < _speakers.Count; i++)
                {
                    _speakers[i].StartPlayingNew(clip);
                }

                playing = true;
            }
        }
    }
}
