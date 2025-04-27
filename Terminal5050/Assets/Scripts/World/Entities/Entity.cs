using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    public static Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
    
    [SerializeField] private string entityId;
    
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource footsteps;
    [SerializeField] private AudioSource growling;
    [SerializeField] private AudioClip[] growlingClips;
    [SerializeField] private float minGrowlingDelay;
    [SerializeField] private float maxGrowlingDelay;

    public bool footstep = false;
    
    public string id => entityId;

    private void Start()
    {
        StartCoroutine(Growling());
        if (NetworkManager.Instance.Server != null)
        {
            agent.SetDestination(player.position);
        }
        entities.Add(entityId, this);
    }
    
    private static System.Random random = new System.Random();

    private string GenerateId()
    {
        string generatedId = RandomString(5);

        while (IdExists(generatedId))
        {
            generatedId = RandomString(5);
        }

        return generatedId;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(entityId))
        {
            entityId = GenerateId();
        }
    }
    
    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private bool IdExists(string id)
    {
        foreach (var entity in FindObjectsOfType<Entity>())
        {
            if (entity.entityId == id)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 _oldPos;

    private void Update()
    {
        if (_oldPos != transform.position)
        {
            // footsteps.volume = 1;
            animator.SetBool("Walking", true);
        }
        else
        {
            // footsteps.volume = 0;
            animator.SetBool("Walking", false);
        }

        _oldPos = transform.position;
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
