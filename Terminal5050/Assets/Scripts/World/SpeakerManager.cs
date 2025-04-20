using System;
using System.Collections.Generic;
using System.Linq;
using Riptide;
using UnityEngine;

public class SpeakerManager : MonoBehaviour
{
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioClip poweringDown;
    [SerializeField] private float speakerPowerDrainPerSecond = 1;
    public static SpeakerManager Instance;

    private List<Speaker> _speakers;

    public bool playing;

    private void Awake()
    {
        Instance = this;

        _speakers = FindObjectsOfType<Speaker>().ToList();
        
        CMDManager.Instance.OnChoiceSelected += OnChoiceSelected;
    }

    private void Update()
    {
        if (playing)
        {
            PowerManager.Instance.ChangeCharge(-speakerPowerDrainPerSecond * Time.deltaTime);
        }
    }

    private void OnChoiceSelected(object sender, string trackName)
    {
        if (sender is SpeakerManager)
        {
            SendSpeakerStartMessage(Player.LocalPlayer.id, trackName);
            
            beep.Play();
        }
    }
    
    private List<string> clipNames = new List<string>();

    public void OpenSpeakerScreen()
    {
        clipNames.Clear();
        List<AudioClip> clips = Resources.LoadAll<AudioClip>("Music").ToList();

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
        
        PowerManager.Instance.RemoveDrain("Speakers");

        playing = false;
    }

    public void StartPlaying(AudioClip clip)
    {
        for (int i = 0; i < _speakers.Count; i++)
        {
            _speakers[i].StartPlayingNew(clip);
        }

        PowerManager.Instance.NewDrain("Speakers", speakerPowerDrainPerSecond);
        
        playing = true;
        
        CMDManager.Instance.Output($"Playing {clip.name}.mp3");
    }

    public void PowerOverload()
    {
        for (int i = 0; i < _speakers.Count; i++)
        {
            _speakers[i].StartPlayingNew(poweringDown);
        }
    }

    #region StartSpeakers

    private void SendSpeakerStartMessage(ushort id, string trackName)
    {
        if (NetworkManager.Instance.Server != null)
        {
            AudioClip selected = Resources.Load<AudioClip>($"Music/{trackName.Replace(".mp3", "")}");
            
            StartPlaying(selected);
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.SpeakersStarted);
            message.AddUShort(id);
            message.AddString(trackName);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.StartSpeakers);
            message.AddString(trackName);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public void ServerSpeakersStarted(ushort client, string trackName)
    {
        SendSpeakerStartMessage(client, trackName);
    }

    public void ClientStartSpeakers(ushort client, string trackName)
    {
        AudioClip selected = Resources.Load<AudioClip>($"Music/{trackName.Replace(".mp3", "")}");
            
        StartPlaying(selected);
    }

    #endregion
    
    #region StopSpeakers

    public void SendSpeakerStopMessage()
    {
        if (NetworkManager.Instance.Server != null)
        {
            StopPlaying();
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.SpeakersStopped);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.StopSpeakers);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public void ServerSpeakersStopped()
    {
        SendSpeakerStopMessage();
    }

    public void ClientStopSpeakers()
    {
        StopPlaying();
    }

    #endregion
}
