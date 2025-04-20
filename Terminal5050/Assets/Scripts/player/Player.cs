using System;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;
    public static UnityEvent localPlayerChanged;

    public ushort id;
    public bool local;

    public string username;

    [HideInInspector] public PlayerRotationManager rotationManager;
    [HideInInspector] public PersonalPowerManager powerManager;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public TorchManager tManager;
    [HideInInspector] public PlayerMovement movement;

    private void Awake()
    {
        rotationManager = GetComponentInChildren<PlayerRotationManager>();
        powerManager = GetComponent<PersonalPowerManager>();
        inventory = GetComponent<Inventory>();
        tManager = GetComponent<TorchManager>();
        movement = GetComponent<PlayerMovement>();
    }

    public static Player SpawnNewPlayer(string username, ushort id, bool local)
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");

        GameObject playerObj = Instantiate(playerPrefab, PlayerSpawningInfo.Instance.SpawnLocation.position, Quaternion.identity, PlayerSpawningInfo.Instance.transform);
        
        Player newPlayer = playerObj.GetComponentInChildren<Player>();
        newPlayer.Init(username, id);
        
        if (local)
        {
            Debug.Log($"Making player {id} local");
            newPlayer.MakeLocal();
        }
        else
        {
            Debug.Log($"Making player {id} not local");
            newPlayer.GetComponentInChildren<MeshRenderer>().gameObject.layer = 12;
        }

        return newPlayer;
    }

    public static void PlayerLeft(ushort id)
    {
        foreach (var player in NetworkManager.Instance.players.Values)
        {
            if (player.id == id)
            {
                Destroy(player.transform.parent.gameObject);
            }
        }
    }

    public void MakeLocal()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.NotLocal();
        }

        ConsoleInteraction[] interactions = FindObjectsOfType<ConsoleInteraction>();

        foreach (var interaction in interactions)
        {
            interaction.SetPlayer(transform.parent.GetComponentInChildren<Camera>().transform, GetComponent<PlayerMovement>());
        }
        
        LocalPlayer = this;
        
        local = true;
        localPlayerChanged?.Invoke();
    }

    private void NotLocal()
    {
        local = false;
    }
    
    private void Init(string name, ushort id)
    {
        if (LocalPlayer == null)
        {
            MakeLocal();
        }

        username = name;
        this.id = id;
    }
}
