using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string id;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip opening;
    [SerializeField] private AudioClip closing;
    [SerializeField] private Transform closedPos;
    [SerializeField] private Transform openPos;
    [SerializeField] private Transform doorObject;
    [SerializeField] private float speed;
    public bool open;
    public int powerLoad = 50;

    private void OnValidate()
    {
        if (!source)
        {
            source = GetComponentInChildren<AudioSource>();
        }
    }

    public void Toggle()
    {
        open = !open;

        if (open)
        {
            source.PlayOneShot(opening);
        }
        else
        {
            source.PlayOneShot(closing);
        }
    }

    private void Update()
    {
        if (open)
        {
            doorObject.position = Vector3.Lerp(doorObject.position, openPos.position, speed * Time.deltaTime);
        }
        else
        {
            doorObject.position = Vector3.Lerp(doorObject.position, closedPos.position, speed * Time.deltaTime);
        }
    }
}
