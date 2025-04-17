using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;
    public static UnityEvent localPlayerChanged;
    
    public bool local;

    public string username;

    public void MakeLocal()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.NotLocal();
        }
        
        LocalPlayer = this;
        
        local = true;
        localPlayerChanged?.Invoke();
        Debug.Log($"Local player changed to {transform.parent.name}");
    }

    private void NotLocal()
    {
        Debug.Log($"Local player is not to {transform.parent.name}");
        local = false;
    }

    private void Start()
    {
        Debug.Log(LocalPlayer);
        Init("PayishVibes");
    }
    
    private void Init(string name)
    {
        Debug.Log(LocalPlayer);
        if (LocalPlayer == null)
        {
            MakeLocal();
        }

        username = name;
    }
}
