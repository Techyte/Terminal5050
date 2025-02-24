using System;
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
            if (PowerManager.Instance.currentLoad + door.powerLoad > PowerManager.Instance.maximumLoad && !door.open)
            {
                foreach (var resetDoor in _doors.Values)
                {
                    resetDoor.open = false;
                    PowerManager.Instance.LoadReduced("Door " + resetDoor.id);
                }

                return;
            }
            
            door.Toggle();
            if (door.open)
            {
                PowerManager.Instance.LoadIncreased("Door " + id, door.powerLoad);
            }
            else
            {
                PowerManager.Instance.LoadReduced("Door " + id);
            }
        }
    }
}
