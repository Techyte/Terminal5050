using UnityEngine;

public class ManualDoorInteract : MonoBehaviour
{
    public string id;
    
    public void Interact()
    {
        ManualDoor[] doors = FindObjectsOfType<ManualDoor>();

        foreach (var door in doors)
        {
            if (door.id == id)
            {
                door.open = !door.open;
            }
        }
    }
}
