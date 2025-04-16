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
        StopAllCoroutines();
        _audioSource.clip = clip;
        _audioSource.Play();
        StartCoroutine(EndAfter(clip.length));
    }

    public void StopPlaying()
    {
        StopAllCoroutines();
        _audioSource.Stop();
    }

    private IEnumerator EndAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("stopping");
        if (SpeakerManager.Instance.playing)
        {
            SpeakerManager.Instance.StopPlaying();
        }
    }
}
