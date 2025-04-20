using System.Collections;
using System.Collections.Generic;
using Riptide;
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

    public void ToggleDoor(string id, bool newState)
    {
        if (_doors.TryGetValue(id, out Door door))
        {
            door.Toggle(newState);
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
                Instance.ToggleDoor(resetDoor.id, false);
            }
        }
    }

    public static void SendDoorToggleMessage(ushort id, string doorId, bool newState)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            Instance.ToggleDoor(doorId, newState);
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.DoorToggled);
            message.AddUShort(id);
            message.AddString(doorId);
            message.AddBool(newState);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.ToggleDoor);
            message.AddString(doorId);
            message.AddBool(newState);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static  void ServerReceivedToggleDoor(ushort id, string doorId, bool newState)
    {
        SendDoorToggleMessage(id, doorId, newState);
    }

    public static  void ClientReceivedDoorToggled(ushort id, string doorId, bool newState)
    {
        Instance.ToggleDoor(doorId, newState);
    }
}
