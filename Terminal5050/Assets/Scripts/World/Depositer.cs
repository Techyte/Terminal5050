using Riptide;
using UnityEngine;

public class Depositer : Interactable
{
    public static Depositer Instance;
    
    private BoxCollider boxCollider;

    private void Awake()
    {
        Instance = this;
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        hoverable = true;
    }

    public override void Interact(Player player)
    {
        if (player.local)
        {
            Item currentItem = player.inventory.heldItem;

            if (currentItem != null)
            {
                if (currentItem.template.canBeSold)
                {
                    SendDepositItemMessage(player.id, player.inventory.largeItem != null, player.inventory.selectedIndex, currentItem.template.sellPrice);
                }
            }
        }
    }

    public override string GetHoverText(Player player)
    {
        return player.inventory.heldItem != null ? $"Deposit {player.inventory.heldItem.template.name}" : "Deposit here";
    }

    public override Vector3 GetHoverBounds()
    {
        return boxCollider.bounds.center;
    }

    private void RemoveItem(ushort id, bool big, int index)
    {
        // remove the item from the players inventory
        if (NetworkManager.Instance.players.TryGetValue(id, out Player player))
        {
            Debug.Log($"Removing item for id {id}");
            if (player.local)
            {
                ActionBar.NewOutput($"Sold {player.inventory.heldItem.template.name}");
            }

            if (big)
            {
                player.inventory.largeItem = null;
            }
            else
            {
                player.inventory.smallItems[index] = null;
            }
            player.inventory.UpdateSelectedItem();
        }
    }

    public static void SendDepositItemMessage(ushort id, bool big, int itemToDepositIndex, int value)
    {
        // host
        if (NetworkManager.Instance.Server != null)
        {
            // add the value of the item
            BankManager.Instance.SetNewValue(BankManager.Instance.Value + value);
            
            // change the value we send to the new total, this is what the clients will receive
            value = BankManager.Instance.Value;
            
            Instance.RemoveItem(id, big, itemToDepositIndex);
            
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientMessageId.ItemDeposited);
            message.AddUShort(id);
            message.AddBool(big);
            message.AddInt(itemToDepositIndex);
            message.AddInt(value);

            NetworkManager.Instance.Server.SendToAll(message, Player.LocalPlayer.id);
        }
        else // client wants to notify the server
        {
            Message message = Message.Create(MessageSendMode.Reliable, ClientToServerMessageId.DepositItem);
            message.AddBool(big);
            message.AddInt(itemToDepositIndex);
            message.AddInt(value);

            NetworkManager.Instance.Client.Send(message);
        }
    }

    public static void ServerReceivedDepositItem(ushort id, bool big, int itemToDepositIndex, int value)
    {
        SendDepositItemMessage(id, big, itemToDepositIndex, value);
    }

    public static void ClientReceivedItemDeposited(ushort id, bool big, int itemToDepositIndex, int value)
    {
        BankManager.Instance.SetNewValue(value);

        Instance.RemoveItem(id, big, itemToDepositIndex);
    }
}
