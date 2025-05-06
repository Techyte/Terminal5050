using System.Collections;
using Riptide;
using TMPro;
using UnityEngine;

public class DoorIndicator : Interactable
{
    [SerializeField] private Door sourceDoor;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float powerCost;

    private void Awake()
    {
        text.text = sourceDoor.id;
    }

    public override void Interact(Player player)
    {
        Inventory inventory = player.inventory;
        if (inventory.heldItem != null)
        {
            if (inventory.heldItem.template.name == "Scanner")
            {
                SendDoorPingMessage(Player.LocalPlayer.id, sourceDoor.id);
            }
            else
            {
                if (player.local)
                {
                    ActionBar.NewOutput("Scanner needed");
                }
            }
        }
        else
        {
            if (player.local)
            {
                ActionBar.NewOutput("Scanner needed");
            }
        }
    }

    private IEnumerator PingDoor(PersonalPowerManager pManager)
    {
        bool local = pManager.GetComponent<Player>().local;

        if (local)
            ActionBar.NewOutput("Attempting to ping door back to base");
        
        Inventory inventory = pManager.GetComponent<Inventory>();
        TorchManager tManager = pManager.GetComponent<TorchManager>();
        
        yield return new WaitForSeconds(1);
        
        if (pManager.charge - tManager.scannerDrain <= 0)
        {
            if (local)
                ActionBar.NewOutput("Insufficient power", Color.red);
            yield break;
        }
        
        pManager.charge -= tManager.scannerDrain;
        
        CMDManager.Instance.tBehaviour.PlayerPingedDoor(sourceDoor.id);
        if (local)
            ActionBar.NewOutput("Ping successful");
        
        inventory.Beep.Play();
    }

    public static void SendDoorPingMessage(ushort id, string doorId)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
            {
                DoorIndicator indicator = Door.FindDoorById(doorId).indicator;
                indicator.StartCoroutine(indicator.PingDoor(player.powerManager));
            }
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.DoorPinged);
            message.AddUShort(id);
            message.AddString(doorId);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.PingDoor);
            message.AddString(doorId);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static  void ServerReceivedToggleItem(ushort id, string doorId)
    {
        SendDoorPingMessage(id, doorId);
    }

    public static  void ClientReceivedItemToggled(ushort id, string doorId)
    {
        if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
        {
            DoorIndicator indicator = Door.FindDoorById(doorId).indicator;
            indicator.StartCoroutine(indicator.PingDoor(player.powerManager));
        }
    }
}
