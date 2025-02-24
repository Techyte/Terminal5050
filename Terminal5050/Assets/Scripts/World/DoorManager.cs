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
            door.Toggle();
        }
    }
}
