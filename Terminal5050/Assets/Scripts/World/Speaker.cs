using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void StartPlayingNew(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void StopPlaying()
    {
        _audioSource.Stop();
    }

    private void Update()
    {
        if (SpeakerManager.Instance.playing && !_audioSource.isPlaying)
        {
            SpeakerManager.Instance.StopPlaying();
        }
    }
}
