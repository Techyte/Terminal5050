using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource footsteps;
    [SerializeField] private AudioSource growling;
    [SerializeField] private AudioClip[] growlingClips;
    [SerializeField] private float minGrowlingDelay;
    [SerializeField] private float maxGrowlingDelay;

    public bool footstep = false;

    private void Start()
    {
        agent.SetDestination(player.position);
        StartCoroutine(Growling());
    }

    private void Update()
    {
        if (agent.velocity.magnitude != 0f)
        {
            // footsteps.volume = 1;
            animator.SetBool("Walking", true);
        }
        else
        {
            // footsteps.volume = 0;
            animator.SetBool("Walking", false);
        }
    }

    private void LateUpdate()
    {
        if (footstep)
        {
            footstep = false;
            footsteps.Play();
        }
    }

    private IEnumerator Growling()
    {
        growling.PlayOneShot(growlingClips[Random.Range(0, growlingClips.Length)]);
        yield return new WaitForSeconds(Random.Range(minGrowlingDelay, maxGrowlingDelay));
        StartCoroutine(Growling());
    }
}
