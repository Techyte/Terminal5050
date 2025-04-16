using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance;

    private Dictionary<string, Door> _doors;

    private void Awake()
    {
        Instance = this;

        _doors = new Dictionary<string, Door>();

        Door[] foundDoors = FindObjectsOfType<Door>();

        foreach (var door in foundDoors)
        {
            _doors.Add(door.id, door);
        }
    }

    public void ToggleDoor(string id)
    {
        if (_doors.TryGetValue(id, out Door door))
        {
            door.Toggle();
            if (door.open)
            {
                PowerManager.Instance.NewDrain($"Door {id}", 5);
                PowerManager.Instance.ChangeCharge(-5);
                StartCoroutine(DisplayDoorDrain(id));
            }
        }
    }

    private IEnumerator DisplayDoorDrain(string doorId)
    {
        yield return new WaitForSeconds(1);
        PowerManager.Instance.RemoveDrain($"Door {doorId}");
    }

    public void CloseAllDoors()
    {
        foreach (var resetDoor in _doors.Values)
        {
            if (resetDoor.open)
            {
                ToggleDoor(resetDoor.id);
            }
        }
    }
}
