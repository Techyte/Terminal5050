using UnityEngine;

public class PlayerSpawningInfo : MonoBehaviour
{
    public static PlayerSpawningInfo Instance;
    
    [SerializeField] private Transform spawnLocation;
    
    public Transform SpawnLocation => spawnLocation;

    private void Awake()
    {
        Instance = this;
    }
}
