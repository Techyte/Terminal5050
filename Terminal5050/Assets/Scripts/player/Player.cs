using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;
    public static UnityEvent localPlayerChanged;
    
    public bool local;

    public string username;

    public static Player SpawnNewPlayer(string username, bool local)
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player");

        GameObject playerObj = Instantiate(playerPrefab, PlayerSpawningInfo.Instance.SpawnLocation.position, Quaternion.identity, PlayerSpawningInfo.Instance.transform);
        
        Player newPlayer = playerObj.GetComponentInChildren<Player>();
        newPlayer.Init(username);
        
        if (local)
        {
            newPlayer.MakeLocal();
        }

        return newPlayer;
    }

    public void MakeLocal()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.NotLocal();
        }
        
        LocalPlayer = this;
        
        local = true;
        localPlayerChanged?.Invoke();
    }

    private void NotLocal()
    {
        local = false;
    }

    private void Start()
    {
        Init("PayishVibes");
    }
    
    private void Init(string name)
    {
        if (LocalPlayer == null)
        {
            MakeLocal();
        }

        username = name;
    }
}
