using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeakerManager : MonoBehaviour
{
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioClip poweringDown;
    public static SpeakerManager Instance;

    private List<Speaker> _speakers;

    public bool playing;

    private void Awake()
    {
        Instance = this;

        _speakers = FindObjectsOfType<Speaker>().ToList();
        
        CMDManager.Instance.OnChoiceSelected += OnChoiceSelected;
    }

    private void OnChoiceSelected(object sender, int e)
    {
        if (sender is SpeakerManager)
        {
            AudioClip selected = clips[e];
            
            StartPlaying(selected);
            
            beep.Play();
        }
    }

    private AudioClip[] clips;
    private List<string> clipNames = new List<string>();

    public void OpenSpeakerScreen()
    {
        clipNames.Clear();
        clips = Resources.LoadAll<AudioClip>("Music");

        foreach (var clip in clips)
        {
            clipNames.Add(clip.name+".mp3");
        }
        
        CMDManager.Instance.OutputChoice(clipNames.ToArray(), "Select Sound to play", this);
    }

    public void StopPlaying()
    {
        for (int i = 0; i < _speakers.Count; i++)
        {
            _speakers[i].StopPlaying();
        }

        playing = false;
        
        PowerManager.Instance.LoadReduced("Speakers");
    }

    public void StartPlaying(AudioClip clip)
    {
        for (int i = 0; i < _speakers.Count; i++)
        {
            _speakers[i].StartPlayingNew(clip);
        }

        playing = true;
        
        CMDManager.Instance.Output($"Playing {clip.name}.mp3");
        
        PowerManager.Instance.LoadIncreased("Speakers", 15);
    }

    public void PowerOverload()
    {
        for (int i = 0; i < _speakers.Count; i++)
        {
            _speakers[i].StartPlayingNew(poweringDown);
        }
    }
}
